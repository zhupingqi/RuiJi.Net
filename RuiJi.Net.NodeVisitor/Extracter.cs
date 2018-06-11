using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extracter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.NodeVisitor
{
    public class Extracter
    {        
        public static List<ExtractResult> Extract(ExtractRequest request)
        {
            var proxyUrl = ProxyManager.Instance.Elect(NodeProxyTypeEnum.Extracter);

            if (string.IsNullOrEmpty(proxyUrl))
                throw new Exception("no available extracter proxy servers");

            var client = new RestClient("http://" + proxyUrl);
            var restRequest = new RestRequest("api/ep/extract");
            restRequest.Method = Method.POST;
            restRequest.JsonSerializer = new NewtonJsonSerializer();

            var json = JsonConvert.SerializeObject(request);

            restRequest.AddJsonBody(json);
            restRequest.Timeout = 15000;

            var restResponse = client.Execute(restRequest);

            var response = JsonConvert.DeserializeObject<List<ExtractResult>>(restResponse.Content);

            return response;
        }
    }
}