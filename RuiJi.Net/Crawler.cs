using Newtonsoft.Json;
using RestSharp;
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
            var baseUrl = ProxyManager.Instance.Elect(ProxyTypeEnum.Crawler);

            var client = new RestClient(baseUrl);
            var restRequest = new RestRequest("api/proxy/request");
            restRequest.Method = Method.POST;
            restRequest.AddJsonBody(request);
            restRequest.Timeout = request.Timeout;

            var restResponse = client.Execute(restRequest);

            var response = JsonConvert.DeserializeObject<Response>(restResponse.Content);
            if (response == null || restResponse.StatusCode != System.Net.HttpStatusCode.OK)
                response = new Response();

            return response;
        }
    }
}