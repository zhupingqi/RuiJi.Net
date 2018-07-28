using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Feed.Db;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.Storage;
using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuiJi.Net.Owin.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/fp")]
    public class FeedProxyController : ControllerBase
    {
        #region Rule
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("rule/list")]
        public object Rules(int offset, int limit, string key, string type, string status)
        {
            var node = ServerManager.Get(Request.Host.Value);

            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            return new
            {
                rows = RuleLiteDb.GetModels(paging, key, type, status),
                total = paging.Count
            };
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("rule/update")]
        public void UpdateRule([FromBody]RuleModel rule)
        {
            RuleLiteDb.AddOrUpdate(rule);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("rule")]
        public object GetRule(int id)
        {
            var feed = RuleLiteDb.Get(id);

            return feed;
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("rule/status")]
        public bool ChangeRuleStatus(string ids, string status)
        {
            var changeIds = ids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
            var statusEnum = (Status)Enum.Parse(typeof(Status), status.ToUpper());

            return RuleLiteDb.StatusChange(changeIds, statusEnum);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("rule/remove")]
        public bool RemoveRule(string ids)
        {
            var removes = ids.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            return RuleLiteDb.Remove(removes);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("rule/match")]
        public object MatchUrlRule(string url, bool useBlock = false)
        {
            if (useBlock)
                return RuleLiteDb.Match(url).Select(m => new ExtractFeatureBlock(JsonConvert.DeserializeObject<ExtractBlock>(m.BlockExpression), m.Feature)).ToList();
            else
            {
                return RuleLiteDb.Match(url).Select(m => new ExtractFeatureBlock(RuiJiBlockParser.ParserBlock(m.RuiJiExpression), m.Feature)).ToList();
            }
        }
        #endregion

        #region Feed
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("feed/list")]
        public object Feeds(int offset, int limit, string key, string method, string type, string status)
        {
            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            return new
            {
                rows = FeedLiteDb.GetFeedModels(paging, key, method, type, status),
                total = paging.Count
            };
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("feed/update")]
        public void UpdateFeed([FromBody]FeedModel feed)
        {
            FeedLiteDb.AddOrUpdate(feed);

            var @event = new BroadcastEvent()
            {
                Event = BroadcastEventEnum.UPDATE,
                Args = feed
            };

            Broadcast(@event);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("feed/status")]
        public bool ChangeFeedStatus(string ids, string status)
        {
            var changeIds = ids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
            var statusEnum = (Status)Enum.Parse(typeof(Status), status.ToUpper());

            if (statusEnum == Status.ON)
            {
                var feeds = FeedLiteDb.GetFeed(changeIds);

                foreach (var feed in feeds)
                {
                    var @event = new BroadcastEvent()
                    {
                        Event = BroadcastEventEnum.UPDATE,
                        Args = feed
                    };

                    Broadcast(@event);
                }
            }
            else
            {
                var @event = new BroadcastEvent()
                {
                    Event = BroadcastEventEnum.REMOVE,
                    Args = changeIds
                };

                Broadcast(@event);
            }

            return FeedLiteDb.ChangeStatus(changeIds, statusEnum);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("feed/remove")]
        public bool RemoveFeed(string ids)
        {
            var removes = ids.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            var @event = new BroadcastEvent()
            {
                Event = BroadcastEventEnum.REMOVE,
                Args = removes
            };

            Broadcast(@event);

            return FeedLiteDb.Remove(removes);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("feed")]
        public object GetFeed(int id)
        {
            var feed = FeedLiteDb.GetFeed(id);

            return feed;
        }

        private void Broadcast(BroadcastEvent @event)
        {
            if (RuiJiConfiguration.Standalone)
            {
                FeedScheduler.Schedulers.First().Value.OnReceive(@event);
            }
            else
            {
                var node = ServerManager.Get(Request.Host.Value) as NodeBase;
                var nv = node.GetChildren("/live_nodes/feed");

                foreach (string path in nv.Keys)
                {
                    var baseUrl = path.Substring(path.LastIndexOf("/") + 1);

                    var client = new RestClient("http://" + baseUrl);
                    var restRequest = new RestRequest("api/feed/change");
                    restRequest.Method = Method.POST;
                    restRequest.JsonSerializer = new NewtonJsonSerializer();
                    restRequest.AddJsonBody(@event);

                    restRequest.Timeout = 15000;

                    client.ExecuteAsync(restRequest,(restResponse)=> { });
                }
            }
        }

        #endregion

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("feed/page")]
        public object GetFeedJob(string pages)
        {
            try
            {
                var ps = pages.Split(',').Select(m => Convert.ToInt32(m)).ToArray();
                var feeds = FeedLiteDb.GetFeedModels(ps, 50);

                return feeds;
            }
            catch
            {
            }

            return new { };
        }

        #region 存储抓取结果
        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("content/save")]
        public bool SaveContent([FromBody]ContentModel content, string shard = "")
        {
            try
            {
                if (string.IsNullOrEmpty(shard))
                    shard = DateTime.Now.ToString("yyyyMM");

                var storage = new LiteDbStorage(@"LiteDb/Content/" + shard + ".db", "contents");
                return storage.Insert(content) != -1;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("content/shards")]
        public object GetShards()
        {
            var dbfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LiteDb", "Content");
            if (!System.IO.File.Exists(dbfile))
            {
                return new List<string> { DateTime.Now.ToString("yyyyMM")};
            }
            var fileInfos = Directory.GetFiles(dbfile);
            var shards = fileInfos.Select(f => f.Substring(f.LastIndexOf("\\") + 1, f.IndexOf(".db") - f.LastIndexOf("\\") - 1)).OrderByDescending(f => f).ToList();
            return shards;
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("content/list")]
        public object GetContent(int offset, int limit, string shard = "", string feedId = "")
        {
            var node = ServerManager.Get(Request.Host.Value);

            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            if (string.IsNullOrEmpty(shard) || shard.ToLower() == "shard")
                shard = DateTime.Now.ToString("yyyyMM");
            var feedIdInt = string.IsNullOrEmpty(feedId) ? 0 : Convert.ToInt32(feedId);
            return new
            {
                rows = ContentLiteDb.GetModels(paging, shard, feedIdInt).Select(m => new
                {
                    id = m.Id,
                    feedId = m.FeedId,
                    url = m.Url,
                    cdate = m.CDate,
                    metas = m.Metas.Select(n => new
                    {
                        name = n.Key,
                        content = n.Value == null ? "" : (n.Value.ToString().Length > 50 ? n.Value.ToString().Substring(0, 50) : n.Value.ToString())
                    })
                }),
                total = paging.Count
            };
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("content/remove")]
        public bool RemoveContent(string ids, string shard)
        {
            var removes = ids.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            return ContentLiteDb.Remove(removes, shard);
        }
        #endregion
    }
}