using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.NodeVisitor
{
    public class Crawler
    {
        public static Response Request(Request request)
        {
            if (RuiJiConfiguration.Standalone)
            {
                if (string.IsNullOrEmpty(request.Ip))
                {
                    var e = CrawlerServerManager.Instance.ElectIP(request.Uri);
                    if (e != null)
                        request.Ip = e.ClientIp;
                    else
                        request.Ip = IPHelper.GetDefaultIPAddress().ToString();
                }

                var crawler = new RuiJiCrawler();
                var response = crawler.Request(request);

                var maxRefresh = 2;
                string refreshUrl;

                while (HasRefreshMeta(response, out refreshUrl) && maxRefresh > 0)
                {
                    crawler = new RuiJiCrawler();
                    request.Uri = new Uri(refreshUrl);
                    response = crawler.Request(request);

                    maxRefresh--;
                }

                return response;
            }
            else
            {
                var proxyUrl = ProxyManager.Instance.Elect(NodeProxyTypeEnum.CRAWLERPROXY);
                if (string.IsNullOrEmpty(proxyUrl))
                    throw new Exception("no available crawler proxy servers");

                proxyUrl = IPHelper.FixLocalUrl(proxyUrl);

                if (!request.Session)
                    request = (Request)request.Clone();

                var elect = Elect(new CrawlerElectRequest
                {
                    ElectIp = string.IsNullOrEmpty(request.Ip),
                    ElectProxy = request.Proxy is null,
                    Uri = request.Uri
                });

                if (request.Proxy is null)
                    request.Proxy = elect.Proxy;

                if (string.IsNullOrEmpty(request.Ip))
                    request.Ip = elect.ClientIp;

                if (string.IsNullOrEmpty(request.Elect))
                    request.Elect = elect.BaseUrl;

                var client = new RestClient("http://" + request.Elect);
                var restRequest = new RestRequest("api/crawler/request");
                restRequest.Method = Method.POST;
                restRequest.AddJsonBody(request);
                restRequest.Timeout = request.Timeout;

                Response response = null;
                var resetEvent = new ManualResetEvent(false);

                var handle = client.ExecuteAsync(restRequest, (restResponse) =>
                {
                    response = JsonConvert.DeserializeObject<Response>(restResponse.Content);

                    resetEvent.Set();
                });

                resetEvent.WaitOne();

                return response;
            }
        }

        private static bool HasRefreshMeta(Response response, out string refreshUrl)
        {
            if (!response.IsRaw)
            {
                var reg = new Regex("<meta[\\s]+http-equiv=\"Refresh\"[\\s]+content=['\"]?[\\d]+;URL=([^'\"]*)['\"]?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var ms = reg.Matches(response.Data.ToString());
                if (ms.Count > 0)
                {
                    refreshUrl = ms[0].Groups[1].Value;
                    if (!Uri.IsWellFormedUriString(refreshUrl, UriKind.Absolute))
                    {
                        refreshUrl = new Uri(response.Request.Uri, refreshUrl).ToString();
                    }

                    return true;
                }
            }
            refreshUrl = "";
            return false;
        }

        public static Response Request(string url, string method = "GET")
        {
            var request = new Request(url);
            request.Method = method;

            return Request(request);
        }

        public static CrawlerElectResult Elect(CrawlerElectRequest request)
        {
            var proxyUrl = "";

            if (RuiJiConfiguration.Standalone)
            {
                proxyUrl = RuiJiConfiguration.RuiJiServer;
            }
            else
            {
                proxyUrl = ProxyManager.Instance.Elect(NodeProxyTypeEnum.CRAWLERPROXY);
            }

            if (string.IsNullOrEmpty(proxyUrl))
                throw new Exception("no available crawler proxy servers");

            proxyUrl = IPHelper.FixLocalUrl(proxyUrl);

            var client = new RestClient("http://" + proxyUrl);
            var restRequest = new RestRequest("api/cp/elect?_=" + DateTime.Now.Ticks);
            restRequest.Method = Method.POST;
            restRequest.AddJsonBody(request);
            restRequest.Timeout = 15000;

            CrawlerElectResult response = null;
            var resetEvent = new ManualResetEvent(false);

            var handle = client.ExecuteAsync(restRequest, (restResponse) =>
            {
                response = JsonConvert.DeserializeObject<CrawlerElectResult>(restResponse.Content);
                resetEvent.Set();
            });

            resetEvent.WaitOne();

            return response;
        }
    }
}