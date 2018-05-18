using Newtonsoft.Json;
using RestSharp;
using RuiJi.Core.Crawler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net
{
    public class Crawler
    {
        public static Response Request(Request request)
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
            if (restResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ProxyManager.Instance.MarkDown(proxyUrl);
            }

            return response;
        }
    }
}