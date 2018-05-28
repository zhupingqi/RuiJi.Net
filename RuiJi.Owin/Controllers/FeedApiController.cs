using Newtonsoft.Json;
using RuiJi.Core.Extracter;
using RuiJi.Core.Utils;
using RuiJi.Core.Utils.Page;
using RuiJi.Node;
using RuiJi.Node.Feed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Owin.Controllers
{
    public class FeedApiController : ApiController
    {
        [HttpGet]
        public object Feeds(int offset, int limit)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.FEEDPROXY)
            {
                var paging = new Paging();
                paging.CurrentPage = (offset / limit) + 1;
                paging.PageSize = limit;

                return new
                {
                    rows = FeedLiteDb.GetFeedModels(paging),
                    total = paging.Count
                };
            }

            return new { };
        }

        [HttpGet]
        public object Rules(int offset, int limit)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.FEEDPROXY)
            {
                var paging = new Paging();
                paging.CurrentPage = (offset / limit) + 1;
                paging.PageSize = limit;

                return new
                {
                    rows = RuleLiteDB.GetRuleModels(paging),
                    total = paging.Count
                };
            }

            return new { };
        }

        [HttpGet]
        public object UrlRule(string url)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.FEEDPROXY)
            {
                return RuleLiteDB.Match(url).Select(m => JsonConvert.DeserializeObject<ExtractBlockCollection>(m.Blocks)).ToList();
            }

            return new { };
        }

        [HttpGet]
        public object FeedJob(string pages)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.FEEDPROXY)
            {
                var ps = pages.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

                return PreProcessUrl(FeedLiteDb.GetFeedModels(ps, 50));
            }

            return new { };
        }

        [HttpGet]
        public string Feed()
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.FEED)
            {
                var d = node.GetData("/config/feed/" + Request.RequestUri.Authority).Data;
                var config = JsonConvert.DeserializeObject<NodeConfig>(d);

                if (config.Pages == null)
                    config.Pages = new int[0];

                return string.Join(",", config.Pages);
            }

            return "";
        }

        [HttpGet]
        public void SetFeed(string pages)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.FEED)
            {
                var path = "/config/feed/" + Request.RequestUri.Authority;

                var data = node.GetData("/config/feed/" + Request.RequestUri.Authority);
                var config = JsonConvert.DeserializeObject<NodeConfig>(data.Data);
                config.Pages = pages.Split(',').Select(m=>Convert.ToInt32(m)).ToArray();

                node.SetData(path, JsonConvert.SerializeObject(config));
            }
        }

        private List<FeedModel> PreProcessUrl(List<FeedModel> feeds)
        {
            foreach (var feed in feeds)
            {
                feed.Url = CompileUrl.Compile(feed.Url);
            }

            return feeds;
        }
    }
}