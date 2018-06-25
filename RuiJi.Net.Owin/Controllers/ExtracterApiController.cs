using Newtonsoft.Json;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Utils;
using RuiJi.Net;
using RuiJi.Net.Node.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using RuiJi.Net.NodeVisitor;

namespace RuiJi.Net.Owin.Controllers
{
    public class ExtractorApiController : ApiController
    {
        [HttpPost]
        public List<ExtractResult> Extract([FromBody]string json)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);
            var request = JsonConvert.DeserializeObject<ExtractRequest>(json);

            if (node.NodeType == Node.NodeTypeEnum.Extractor)
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