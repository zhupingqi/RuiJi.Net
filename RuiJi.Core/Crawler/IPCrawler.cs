using RuiJi.Core;
using RuiJi.Core.Cookie;
using RuiJi.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Crawler
{
    public class IPCrawler
    {
        private static List<string> ignoreHeader = new List<string>() {
            "Host", "Connection"
        };

        private IPAddress ip;

        public IPCrawler(IPAddress ip)
        {
            if (!IPHelper.IsHostIPAddress(ip))
            {
                throw new Exception("无效IP地址");
            }
            this.ip = ip;
        }

        public IPCrawler(string ip) : this(IPAddress.Parse(ip))
        {

        }

        public Response Request(Request request)
        {
            var httpResponse = GetHttpWebResponse(request);
            var buff = GetResponseBuff(httpResponse);

            var response = new Response();

            response.StatusCode = httpResponse.StatusCode;
            response.Headers = httpResponse.Headers;
            response.RequestUri = request.Uri;
            response.ResponseUri = httpResponse.ResponseUri;
            response.IsRaw = request.IsRaw;
            response.Method = request.Method;

            IpCookieManager.Instance.Update(request.Ip, request.Uri.ToString(), httpResponse.Headers.Get("setCookie"));

            if (request.IsRaw)
            {
                response.Data = buff;
            }
            else
            {
                var result = Decoding.GetStringFromBuff(buff, httpResponse, request.Charset);
                response.Charset = result.CharSet;
                response.Data = result.Body;
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

            var cookie = string.IsNullOrEmpty(request.Cookie) ? IpCookieManager.Instance.Get(request.Ip, request.Uri.ToString()) : request.Cookie;

            if (!string.IsNullOrEmpty(cookie))
            {
                httpRequest.Headers.Add("Cookie", cookie);
            }

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

            httpRequest.ServicePoint.BindIPEndPointDelegate = (ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount) =>
            {
                if (retryCount > 3)
                {
                    return null;
                }
                return new IPEndPoint(ip, 0);
            };

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

        private void PreprocessHeader(HttpWebRequest request, WebHeaderCollection headers)
        {
            if (headers == null || headers.Count == 0)
                return;

            foreach (string key in headers.Keys)
            {
                if (ignoreHeader.Exists(m => m == key))
                    continue;

                var value = headers[key];

                switch (key) {
                    case "Referer":
                        {
                            request.Referer = value;
                            break;
                        }
                    case "User-Agent":
                        {
                            request.UserAgent = value;
                            break;
                        }
                    case "Accept":
                        {
                            request.Accept = value;
                            break;
                        }
                    case "Content-Type":
                        {
                            request.ContentType = value;
                            break;
                        }
                    default: {
                            request.Headers.Remove(key);
                            if (!string.IsNullOrEmpty(value))
                            {
                                request.Headers.Add(key, value);
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
