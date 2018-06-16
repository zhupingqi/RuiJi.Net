using RuiJi.Net.Core.Cookie;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Log;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    public class RuiJiCrawler
    {
        private static List<string> ignoreHeader = new List<string>() {
            "Host", "Connection"
        };

        public RuiJiCrawler()
        {

        }

        public Response Request(Request request)
        {
            Logger.GetLogger(request.Elect).Info("request " + request.Uri.ToString() + " with ip:" + request.Ip + (request.Proxy != null ? (" proxy:" + request.Proxy.Host + ":" + request.Proxy.Port) : ""));

            if (request.RunJS)
            {
                var p = new PhantomCrawler();
                var res = p.Request(request);
                if (request.UseCookie && res.Headers != null)
                {
                    var cookies = res.Headers.Where(m => m.Name == "Set-Cookie").Select(m => m.Value).ToList();
                    if (cookies.Count > 0)
                    {
                        var c = string.Join("", cookies).Replace("\n",",");
                        SetCookie(request, c);
                        res.Cookie = GetCookie(request);
                    }
                }

                res.ElectInfo = request.Elect;
                res.RequestUri = request.Uri;
                res.Method = request.Method;
                if (res.Proxy != null)
                    res.Proxy = request.Proxy.Host;

                return res;
            }

            if (!string.IsNullOrEmpty(request.Ip))
            {
                if (!IPHelper.IsHostIPAddress(IPAddress.Parse(request.Ip)))
                {
                    return new Response
                    {
                        IsRaw = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Data = "specified Ip is invalid!"
                    };
                }
            }

            var httpResponse = GetHttpWebResponse(request);
            if (httpResponse == null)
            {
                var r = new Response();
                r.StatusCode = HttpStatusCode.BadRequest;
                r.Data = "httpResponse is null";

                return r;
            }

            var buff = GetResponseBuff(httpResponse);

            var response = new Response();

            response.StatusCode = httpResponse.StatusCode;
            response.Headers = WebHeader.FromWebHeader(httpResponse.Headers);
            response.RequestUri = request.Uri;
            response.ResponseUri = httpResponse.ResponseUri;
            response.Method = request.Method;

            if (!string.IsNullOrEmpty(httpResponse.ContentType))
                response.IsRaw = MimeDetect.IsRaw(httpResponse.ContentType);
            else
                response.IsRaw = MimeDetect.IsRaw(httpResponse.ResponseUri);

            if (request.UseCookie)
                SetCookie(request, httpResponse.Headers.Get("Set-Cookie"));

            if (response.IsRaw)
            {
                response.Data = buff;
            }
            else
            {
                var result = Decoding.GetStringFromBuff(buff, httpResponse, request.Charset);
                response.Charset = result.CharSet;
                response.Data = result.Body;
                response.Cookie = GetCookie(request);
            }
            httpResponse.Close();

            return response;
        }

        private HttpWebResponse GetHttpWebResponse(Request request)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(request.Uri);
            httpRequest.ServicePoint.ConnectionLimit = int.MaxValue;
            httpRequest.Method = request.Method;
            httpRequest.MaximumAutomaticRedirections = 3;
            httpRequest.Timeout = request.Timeout > 0 ? request.Timeout : 100000;
            httpRequest.ReadWriteTimeout = 60000;

            if (httpRequest.Method == "POST" && !string.IsNullOrEmpty(request.PostParam))
            {
                byte[] bs = Encoding.ASCII.GetBytes(request.PostParam);
                if (string.IsNullOrEmpty(httpRequest.ContentType))
                    httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.ContentLength = bs.Length;
                using (Stream requestStream = httpRequest.GetRequestStream())
                {
                    requestStream.Write(bs, 0, bs.Length);
                    requestStream.Close();
                }
            }

            SimulateBrowser(httpRequest);
            PreprocessHeader(httpRequest, request.Headers);

            var cookie = GetCookie(request);

            if (!string.IsNullOrEmpty(cookie) && request.UseCookie)
            {
                httpRequest.Headers.Add("Cookie", cookie);
            }

            if (request.Proxy != null && !String.IsNullOrEmpty(request.Proxy.Host + request.Proxy.Port))
            {
                var proxy = new WebProxy(request.Proxy.Host, request.Proxy.Port);

                if (!string.IsNullOrEmpty(request.Proxy.Username + request.Proxy.Password))
                {
                    proxy.Credentials = new NetworkCredential(request.Proxy.Username, request.Proxy.Password);
                }

                httpRequest.Proxy = proxy;
            }

            if (!string.IsNullOrEmpty(request.Ip))
            {
                httpRequest.ServicePoint.BindIPEndPointDelegate = (ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount) =>
                {
                    if (retryCount > 3)
                    {
                        return null;
                    }
                    return new IPEndPoint(IPAddress.Parse(request.Ip), 0);
                };
            }

            try
            {
                return (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                return (HttpWebResponse)ex.Response;
            }
        }

        private byte[] GetResponseBuff(HttpWebResponse response)
        {
            var responseStream = response.GetResponseStream();
            var destination = new MemoryStream();
            responseStream.CopyTo(destination);
            responseStream.Close();

            var buff = new byte[destination.Length];
            buff = destination.ToArray();
            destination.Close();

            return buff;
        }

        private string GetCookie(Request request)
        {
            if (!string.IsNullOrEmpty(request.Cookie))
                return request.Cookie;

            var ip = request.Ip;
            if (string.IsNullOrEmpty(request.Ip))
            {
                ip = IPHelper.GetDefaultIPAddress().ToString();
            }

            return IpCookieManager.Instance.GetCookieHeader(ip, request.Uri.ToString());
        }

        private void SetCookie(Request request, string setCookie)
        {
            if (string.IsNullOrEmpty(setCookie))
                return;

            var ip = request.Ip;
            if (string.IsNullOrEmpty(request.Ip))
            {
                ip = IPHelper.GetDefaultIPAddress().ToString();
            }

            IpCookieManager.Instance.UpdateCookie(ip, request.Uri.ToString(), setCookie);
        }

        private void PreprocessHeader(HttpWebRequest request, List<WebHeader> headers)
        {
            if (headers == null || headers.Count == 0)
                return;

            foreach (var header in headers)
            {
                if (ignoreHeader.Exists(m => m == header.Name))
                    continue;

                switch (header.Name)
                {
                    case "Referer":
                        {
                            request.Referer = header.Value;
                            break;
                        }
                    case "User-Agent":
                        {
                            request.UserAgent = header.Value;
                            break;
                        }
                    case "Accept":
                        {
                            request.Accept = header.Value;
                            break;
                        }
                    case "Content-Type":
                        {
                            request.ContentType = header.Value;
                            break;
                        }
                    default:
                        {
                            request.Headers.Remove(header.Name);
                            if (!string.IsNullOrEmpty(header.Value))
                            {
                                request.Headers.Add(header.Name, header.Value);
                            }
                            break;
                        }
                }
            }
        }

        private void SimulateBrowser(HttpWebRequest request)
        {
            request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
        }
    }
}
