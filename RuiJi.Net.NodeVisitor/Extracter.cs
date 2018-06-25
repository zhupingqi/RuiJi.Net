using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.NodeVisitor
{
    public class Extractor
    {
        public static List<ExtractResult> Extract(ExtractRequest request)
        {
            if (NodeConfigurationSection.Alone)
            {
                var result = RuiJiExtractor.Extract(request);
                return result;
            }
            else
            {
                var proxyUrl = ProxyManager.Instance.Elect(NodeProxyTypeEnum.FEEDPROXY);

                if (string.IsNullOrEmpty(proxyUrl))
                    throw new Exception("no available Extractor proxy servers");

                proxyUrl = IPHelper.FixLocalUrl(proxyUrl);

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
}