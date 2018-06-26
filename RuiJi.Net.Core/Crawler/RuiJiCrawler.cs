using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuiJi.Net.Core.Cookie;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Log;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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
            Logger.GetLogger(request.Elect).Info("request " + request.Uri.ToString() + " with ip:" + request.Ip + (request.Proxy != null ? (" proxy:" + request.Proxy.Ip + ":" + request.Proxy.Port) : ""));

            SimulateBrowser(request);

            try
            {
                if (request.RunJS)
                {
                    var p = new PhantomCrawler();
                    var res = p.Request(request);
                    if (request.UseCookie && res.Headers != null)
                    {
                        var cookies = res.Headers.Where(m => m.Name == "Set-Cookie").Select(m => m.Value).ToList();
                        if (cookies.Count > 0)
                        {
                            var c = string.Join("", cookies).Replace("\n", ",");
                            SetCookie(request, c);
                            res.Cookie = GetCookie(request);
                        }
                    }

                    res.ElectInfo = request.Elect;
                    res.Request = request;
                    res.Method = request.Method;
                    if (res.Proxy != null)
                        res.Proxy = request.Proxy.Ip;

                    Logger.GetLogger(request.Elect).Info(request.Uri.ToString() + " response status is " + res.StatusCode.ToString());

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
                response.Request = request;
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

                Logger.GetLogger(request.Elect).Info(request.Uri.ToString() + " response status is " + response.StatusCode.ToString());

                return response;
            }
            catch (Exception ex)
            {
                var r = new Response();
                r.StatusCode = HttpStatusCode.BadRequest;
                r.Data = "response error " + ex.Message;

                return r;
            }
        }

        private HttpWebResponse GetHttpWebResponse(Request request)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(request.Uri);
            httpRequest.ServicePoint.ConnectionLimit = int.MaxValue;
            httpRequest.Method = request.Method;
            httpRequest.MaximumAutomaticRedirections = 3;
            httpRequest.Timeout = request.Timeout > 0 ? request.Timeout : 100000;
            httpRequest.ReadWriteTimeout = request.Timeout > 0 ? request.Timeout : 100000;
            httpRequest.ContinueTimeout = request.Timeout > 0 ? request.Timeout : 100000;

            if (httpRequest.Method == "POST" && request.Data != null)
            {
                byte[] bs;

                var jObj = new JObject(request.Data);
                if (jObj.Type == JTokenType.String)
                    bs = Encoding.ASCII.GetBytes(request.Data.ToString());
                else
                    bs = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request.Data));

                if (string.IsNullOrEmpty(httpRequest.ContentType))
                    httpRequest.ContentType = "application/x-www-form-urlencoded";

                httpRequest.ContentLength = bs.Length;

                using (Stream requestStream = httpRequest.GetRequestStream())
                {
                    requestStream.Write(bs, 0, bs.Length);
                    requestStream.Close();
                }
            }

            PreprocessHeader(httpRequest, request.Headers);

            var cookie = GetCookie(request);

            if (!string.IsNullOrEmpty(cookie) && request.UseCookie)
            {
                httpRequest.Headers.Add("Cookie", cookie);
            }

            if (request.Proxy != null && !String.IsNullOrEmpty(request.Proxy.Ip + request.Proxy.Port))
            {
                var proxy = new WebProxy(request.Proxy.Ip, request.Proxy.Port);
                //proxy.Address = new Uri(request.Proxy.Host + ":" + request.Proxy.Port);

                if (!string.IsNullOrEmpty(request.Proxy.Username + request.Proxy.Password))
                {
                    proxy.Credentials = new NetworkCredential(request.Proxy.Username, request.Proxy.Password);
                }

                if (request.Proxy.Scheme == "https")
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
                    {
                        return true;
                    });
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

            var task = Task.Factory.StartNew<HttpWebResponse>(() =>
            {
                try
                {
                    return (HttpWebResponse)httpRequest.GetResponse();

                }
                catch (WebException ex)
                {
                    return (HttpWebResponse)ex.Response;
                }
            });

            task.Wait(request.Timeout > 0 ? request.Timeout : 100000);

            if (task.IsCompleted)
                return task.Result;
            else
            {
                task.Dispose();
                return null;
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

            var ua = request.Headers.SingleOrDefault(m => m.Name == "User-Agent").Value;

            return IpCookieManager.Instance.GetCookieHeader(ip, request.Uri.ToString(), ua);
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

            var ua = request.Headers.SingleOrDefault(m => m.Name == "User-Agent").Value;

            IpCookieManager.Instance.UpdateCookie(ip, ua, request.Uri.ToString(), setCookie);
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

        private void SimulateBrowser(Request request)
        {
            if (request.Headers.Count(m => m.Name == "Accept-Encoding") == 0)
                request.Headers.Add(new WebHeader("Accept-Encoding", "gzip, deflate, sdch"));

            if (request.Headers.Count(m => m.Name == "Accept-Language") == 0)
                request.Headers.Add(new WebHeader("Accept-Language", "zh-CN,zh;q=0.8"));

            if (request.Headers.Count(m => m.Name == "Accept") == 0)
                request.Headers.Add(new WebHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"));

            if (request.Headers.Count(m => m.Name == "User-Agent") == 0)
                request.Headers.Add(new WebHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36"));
        }
    }
}
