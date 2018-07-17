using Newtonsoft.Json;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Feed.LTS;
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
        [Route("get")]
        public string GetFeedPage(string baseUrl)
        {
            var node = ServerManager.ZkNode();

            var d = node.GetData("/config/feed/" + baseUrl).Data;
            var config = JsonConvert.DeserializeObject<NodeConfig>(d);

            if (config.Pages == null)
                config.Pages = new int[0];

            return string.Join(",", config.Pages);
        }

        [HttpPost]
        [Route("set")]
        public void SetFeedPage([FromBody]string pages, [FromUri]string baseUrl)
        {
            var node = ServerManager.ZkNode();

            var path = "/config/feed/" + baseUrl;

            var data = node.GetData(path);
            var config = JsonConvert.DeserializeObject<NodeConfig>(data.Data);
            config.Pages = string.IsNullOrEmpty(pages) ? new int[] { } : pages.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            node.SetData(path, JsonConvert.SerializeObject(config));

            FeedScheduler.SyncFeed();
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEED)]
        [Route("change")]
        public void Change([FromBody]BroadcastEvent @event)
        {
            FeedScheduler.OnReceive(@event);
        }
        #endregion
    }
}