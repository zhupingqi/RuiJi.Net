using RuiJi.Core.Crawler;
using RuiJi.Node.CrawlerProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Owin.Controllers
{
    public class CrawlerProxyApiController : ApiController
    {
        [HttpPost]
        [WebApiCacheAttribute(Duration = 10)]
        public new Response Request(Request request)
        {
            var result = CrawlerManager.Instance.ElectIP(request.Uri);
            request.Ip = result.ClientIp;

            return null;
        }
    }
}
