using Microsoft.AspNetCore.Mvc;
using RuiJi.Net.Core.Utils.Logging;

namespace RuiJi.Net.Owin.Controllers
{
    [Route("api/logger")]
    public class LoggerController : ControllerBase
    {
        [HttpGet]
        [Route("log")]
        public object Log()
        {
            var baseUrl = Request.Host.Value;
            var node = ServerManager.Get(baseUrl);

            return MemoryAppender.GetMessage(node.BaseUrl);
        }
    }
}