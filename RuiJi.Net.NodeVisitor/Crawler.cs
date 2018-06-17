using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Crawler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.NodeVisitor
{
    public class Crawler
    {
        public static Response Request(Request request, bool usecp = false)
        {
            var proxyUrl = ProxyManager.Instance.Elect(NodeProxyTypeEnum.CRAWLERPROXY);

            if (string.IsNullOrEmpty(proxyUrl))
                throw new Exception("no available crawler proxy servers");

            if (usecp)
            {
                var client = new RestClient("http://" + proxyUrl);
                var restRequest = new RestRequest("api/cp/crawl");
                restRequest.Method = Method.POST;
                restRequest.AddJsonBody(request);
                restRequest.Timeout = request.Timeout;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<Response>(restResponse.Content);

                return response;
            }
            else
            {
                var elect = Elect(new CrawlerElectRequest
                {
                    ElectIp = true,
                    ElectProxy = true,
                    Uri = request.Uri
                });

                request.Proxy = elect.Proxy;
                request.Elect = elect.BaseUrl;
                if(string.IsNullOrEmpty(request.Ip))
                    request.Ip = elect.ClientIp;

                var client = new RestClient("http://" + elect.BaseUrl);
                var restRequest = new RestRequest("api/crawl");
                restRequest.Method = Method.POST;
                restRequest.AddJsonBody(request);
                restRequest.Timeout = request.Timeout;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<Response>(restResponse.Content);

                if (elect.Proxy != null)
                {
                    response.Proxy = request.Proxy.Host;
                }

                return response;
            }
        }

        public static Response Request(string url, string method = "GET", bool usecp = false)
        {
            var request = new Request(url);
            request.Method = method;

            return Request(request, usecp);
        }

        public static CrawlerElectResult Elect(CrawlerElectRequest request)
        {
            var proxyUrl = ProxyManager.Instance.Elect(NodeProxyTypeEnum.CRAWLERPROXY);

            if (string.IsNullOrEmpty(proxyUrl))
                throw new Exception("no available crawler proxy servers");

            var client = new RestClient("http://" + proxyUrl);
            var restRequest = new RestRequest("api/cp/elect?_=" + DateTime.Now.Ticks);
            restRequest.Method = Method.POST;
            restRequest.AddJsonBody(request);
            restRequest.Timeout = 15000;

            var restResponse = client.Execute(restRequest);

            var response = JsonConvert.DeserializeObject<CrawlerElectResult>(restResponse.Content);

            return response;
        }
    }
}