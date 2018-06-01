﻿using Newtonsoft.Json;
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
        public object UrlRule(string url,bool useBlock = false)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.FEEDPROXY)
            {
                if (useBlock)
                    return RuleLiteDB.Match(url).Select(m => JsonConvert.DeserializeObject<ExtractBlock>(m.BlockExpression)).ToList();
                else
                {
                    return RuleLiteDB.Match(url).Select(m => RuiJiExtracter.PaserBlock(m.RuiJiExpression)).ToList();
                }
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
        public string GetFeedPage()
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

        [HttpPost]
        public void SetFeedPage([FromBody]string pages)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.FEED)
            {
                var path = "/config/feed/" + Request.RequestUri.Authority;

                var data = node.GetData("/config/feed/" + Request.RequestUri.Authority);
                var config = JsonConvert.DeserializeObject<NodeConfig>(data.Data);
                config.Pages = pages.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

                node.SetData(path, JsonConvert.SerializeObject(config));
            }
        }

        [HttpPost]
        public void UpdateFeed(FeedModel feed)
        {
            FeedLiteDb.AddOrUpdate(feed);
        }

        [HttpGet]
        public object GetFeed(int id)
        {
            var feed = FeedLiteDb.GetFeed(id);

            return feed;
        }

        [HttpPost]
        public void UpdateRule(RuleModel rule)
        {
            RuleLiteDB.AddOrUpdate(rule);
        }

        [HttpGet]
        public object GetRule(int id)
        {
            var feed = RuleLiteDB.GetRule(id);

            return feed;
        }

        private List<FeedModel> PreProcessUrl(List<FeedModel> feeds)
        {
            foreach (var feed in feeds)
            {
                var r = CompileUrl.Extract(feed.Address);
                var code = "";
                if (r != null && r.Function == "now")
                {
                    code = string.Format("result = Datetime.Now.ToString({0})", r.Args);
                    feed.Address = CompileUrl.Compile(code);
                }
            }

            feeds.RemoveAll(m => string.IsNullOrEmpty(m.Address));

            return feeds;
        }
    }
}