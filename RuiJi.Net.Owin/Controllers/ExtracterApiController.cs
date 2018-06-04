using Newtonsoft.Json;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extracter;
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
    public class ExtracterApiController : ApiController
    {
        [HttpPost]
        public ExtractResult Extract([FromBody]string json)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);
            var request = JsonConvert.DeserializeObject<ExtractRequest>(json);

            if (node.NodeType == Node.NodeTypeEnum.EXTRACTER)
            {
                var result = RuiJiExtracter.Extract(request.Content, request.Block);
                return result;
            }
            else
            {
                return Extracter.Extract(request);
            }
        }
    }
}