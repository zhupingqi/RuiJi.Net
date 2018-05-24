using Newtonsoft.Json;
using RuiJi.Core;
using RuiJi.Core.Crawler;
using RuiJi.Core.Extensions;
using RuiJi.Core.Extracter;
using RuiJi.Core.Utils;
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
            var ext = new RuiJiExtracter();
            var request = JsonConvert.DeserializeObject<ExtractRequest>(json);

            var result = ext.Extract(request.Content, request.Block);
            return result;
        }
    }
}