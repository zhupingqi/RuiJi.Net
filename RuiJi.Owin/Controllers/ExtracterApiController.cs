using Newtonsoft.Json;
using RuiJi.Core;
using RuiJi.Core.Crawler;
using RuiJi.Core.Extensions;
using RuiJi.Core.Extracter;
using RuiJi.Core.Utils;
using RuiJi.Net;
using RuiJi.Node.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Owin.Controllers
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
                var ext = new RuiJiExtracter();

                var result = ext.Extract(request.Content, request.Block);
                return result;
            }
            else
            {
                return Extracter.Extract(request);
            }
        }
    }
}