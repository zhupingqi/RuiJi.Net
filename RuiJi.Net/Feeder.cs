using Newtonsoft.Json;
using RestSharp;
using RuiJi.Core.Extensions;
using RuiJi.Core.Extracter;
using RuiJi.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net
{
    public class Feeder
    {
        public static List<ExtractBlockCollection> GetExtractBlock(string url)
        {
            var proxyUrl = ProxyManager.Instance.Elect(ProxyTypeEnum.Feed);

            if (string.IsNullOrEmpty(proxyUrl))
                throw new Exception("no available extracter proxy servers");

            proxyUrl = IPHelper.FixLocalUrl(proxyUrl);

            var client = new RestClient("http://" + proxyUrl);
            var restRequest = new RestRequest("api/fp/rule?url=" + url);
            restRequest.Method = Method.GET;
            restRequest.JsonSerializer = new NewtonJsonSerializer();

            restRequest.Timeout = 15000;

            var restResponse = client.Execute(restRequest);

            var response = JsonConvert.DeserializeObject<List<ExtractBlockCollection>>(restResponse.Content);

            return response;
        }
    }
}