using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.NodeVisitor;
using System.Collections.Generic;

namespace RuiJi.Net.Owin.Controllers
{
    [Route("api/extractor")]
    public class ExtractorController : ControllerBase
    {
        [HttpPost]
        [Route("extract")]
        public List<ExtractResult> Extract([FromBody]string json)
        {
            var node = ServerManager.Get(Request.Host.Value);
            var request = JsonConvert.DeserializeObject<ExtractRequest>(json);

            if (node.NodeType == Node.NodeTypeEnum.EXTRACTOR)
            {
                var result = RuiJiExtractor.Extract(request);
                return result;
            }
            else
            {
                return Extractor.Extract(request);
            }
        }
    }
}