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
    public class ExtracterProxyApiController : ApiController
    {
        [HttpGet]
        public bool Ping()
        {
            return true;
        }
    }
}
