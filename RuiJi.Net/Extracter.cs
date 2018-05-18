using Newtonsoft.Json;
using RestSharp;
using RuiJi.Core.Extensions;
using RuiJi.Core.Extracter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net
{
    public class Extracter
    {        
        public static ExtractResult Extract(ExtractRequest request)
        {
            var proxyUrl = ProxyManager.Instance.Elect(ProxyTypeEnum.Extracter);

            if (string.IsNullOrEmpty(proxyUrl))
                throw new Exception("no available extracter proxy servers");

            var client = new RestClient("http://" + proxyUrl);
            var restRequest = new RestRequest("api/ep/extract");
            restRequest.Method = Method.POST;
            restRequest.JsonSerializer = new NewtonJsonSerializer();

            var setting = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            var json = JsonConvert.SerializeObject(request, setting);


            restRequest.AddJsonBody(json);
            restRequest.Timeout = 15000;

            var restResponse = client.Execute(restRequest);

            var response = JsonConvert.DeserializeObject<ExtractResult>(restResponse.Content);
            if (restResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ProxyManager.Instance.MarkDown(proxyUrl);
            }

            return response;
        }
    }
}