using RuiJi.Net.Core.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Net.Owin.Controllers
{
    [RoutePrefix("api/logger")]
    public class LoggerController : ApiController
    {
        [HttpGet]
        [Route("log")]
        public object Log()
        {
            var baseUrl = Request.RequestUri.Authority;
            var node = ServerManager.Get(baseUrl);

            return MemoryAppender.GetMessage(node.BaseUrl);
        }
    }
}