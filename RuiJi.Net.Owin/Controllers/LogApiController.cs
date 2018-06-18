using RuiJi.Net.Core.Utils.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Net.Owin.Controllers
{
    public class LogApiController : ApiController
    {
        [HttpGet]
        public object Log()
        {
            var baseUrl = Request.RequestUri.Authority;
            var node = ServerManager.Get(baseUrl);

            return MemoryAppender.GetMessage(node.BaseUrl);
        }
    }
}