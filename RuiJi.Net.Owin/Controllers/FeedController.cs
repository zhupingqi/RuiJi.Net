using Newtonsoft.Json;
using RuiJi.Net.Node;
using System;
using System.Linq;
using System.Web.Http;

namespace RuiJi.Net.Owin.Controllers
{
    [RoutePrefix("api/feed")]
    public class FeedController : ApiController
    {
        #region Feed
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEED, RouteArgumentName = "baseUrl")]
        [Route("get")]
        public string GetFeedPage(string baseUrl)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var d = node.GetData("/config/feed/" + Request.RequestUri.Authority).Data;
            var config = JsonConvert.DeserializeObject<NodeConfig>(d);

            if (config.Pages == null)
                config.Pages = new int[0];

            return string.Join(",", config.Pages);
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEED, RouteArgumentName = "baseUrl")]
        [Route("set")]
        public void SetFeedPage([FromBody]string pages, [FromUri]string baseUrl)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var path = "/config/feed/" + Request.RequestUri.Authority;

            var data = node.GetData("/config/feed/" + Request.RequestUri.Authority);
            var config = JsonConvert.DeserializeObject<NodeConfig>(data.Data);
            config.Pages = string.IsNullOrEmpty(pages) ? new int[] { } : pages.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            node.SetData(path, JsonConvert.SerializeObject(config));
        }
        #endregion
    }
}