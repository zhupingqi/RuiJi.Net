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
        public Crawler()
        {

        }

        public Response Request(Request request)
        {
            var proxyUrl = ProxyManager.Instance.Elect(ProxyTypeEnum.Crawler);

            if (string.IsNullOrEmpty(proxyUrl))
                throw new Exception("no available crawler proxy servers");

            var client = new RestClient("http://" + proxyUrl);
            var restRequest = new RestRequest("api/cp/crawl");
            restRequest.Method = Method.POST;
            restRequest.AddJsonBody(request);
            restRequest.Timeout = request.Timeout;

            var restResponse = client.Execute(restRequest);

            var response = JsonConvert.DeserializeObject<Response>(restResponse.Content);

            return response;
        }

        public Response Request(string url,string method = "GET")
        {
            var request = new Request(url);
            request.Method = method;

            return Request(request);
        }
    }
}