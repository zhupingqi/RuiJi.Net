using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Extractor;
using RuiJi.Net.NodeVisitor;
using System.Collections.Generic;
using System.Threading;

namespace RuiJi.Net.Owin.Controllers
{
    [Route("api/ep")]
    public class ExtractorProxyController : ControllerBase
    {
        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.EXTRACTORPROXY)]
        [Route("extract")]
        public List<ExtractResult> Extract([FromBody]string json)
        {
            var node = ServerManager.Get(Request.Host.Value);

            if (node.NodeType == Node.NodeTypeEnum.EXTRACTORPROXY)
            {

                var result = ExtractorManager.Instance.Elect();
                if (result == null)
                    return new List<ExtractResult>();

                var client = new RestClient("http://" + result.BaseUrl);
                var restRequest = new RestRequest("api/extractor/extract");
                restRequest.Method = Method.POST;
                restRequest.JsonSerializer = new NewtonJsonSerializer();
                restRequest.AddJsonBody(json);
                restRequest.Timeout = 15000;

                List<ExtractResult> response = null;
                var resetEvent = new ManualResetEvent(false);

                var handle = client.ExecuteAsync(restRequest, (restResponse) => {
                    response = JsonConvert.DeserializeObject<List<ExtractResult>>(restResponse.Content);
                    resetEvent.Set();
                });

                resetEvent.WaitOne();
                return response;
            }
            else
            {
                var request = JsonConvert.DeserializeObject<ExtractRequest>(json);
                return Extractor.Extract(request);
            }
        }
    }
}
