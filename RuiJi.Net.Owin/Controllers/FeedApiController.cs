using Newtonsoft.Json;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Core.Utils.Tasks;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Feed;
using RuiJi.Net.Node.Feed.LTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Net.Owin.Controllers
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
                    return RuleLiteDB.Match(url).Select(m => RuiJiExpression.PaserBlock(m.RuiJiExpression)).ToList();
                }
            }

            return new { };
        }

        [HttpGet]
        public object FeedJob(string pages)
        {
            try
            {

                var node = ServerManager.Get(Request.RequestUri.Authority);

                if (node.NodeType == Node.NodeTypeEnum.FEEDPROXY)
                {
                    var ps = pages.Split(',').Select(m => Convert.ToInt32(m)).ToArray();
                    var feeds = FeedLiteDb.GetFeedModels(ps, 50);
                    feeds.RemoveAll(m => m.Status == FeedStatus.OFF);

                    return PreProcessUrl(feeds);
                }
            }
            catch { }

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

        [HttpGet]
        public object RunCrawl([FromUri]CrawlTaskModel crawlTask)
        {
            ParallelTask task;

            if (crawlTask.TaskId == 0)
            {
                task = ParallelTaskManager.StartNew<CrawlTaskFunc>(crawlTask.FeedId);
            }
            else
            {
                task = ParallelTaskManager.Get(crawlTask.TaskId);
            }

            if (task.Wait(1000))
            {
                return new
                {
                    completed = task.IsCompleted,
                    state = task.ProgressState,
                    taskId = task.TaskId,
                    result = task.Task.Result
                };
            }
            else
            {
                return new
                {
                    completed = task.IsCompleted,
                    state = task.ProgressState,
                    taskId = task.TaskId
                };
            }
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

    public class CrawlTaskModel
    {
        [JsonProperty("feedId")]
        public int FeedId { get; set; }

        [JsonProperty("taskId")]
        public int TaskId { get; set; }
    }

    public class CrawlTaskFunc : IParallelTaskFunc
    {
        public object Run(object t, ParallelTask task)
        {
            var results = new List<ExtractResult>();
            var reporter = task.Progress as IProgress<string>;

            reporter.Report("正在读取Feed记录");
            var feed = FeedLiteDb.GetFeed(Convert.ToInt32(t));

            reporter.Report("正在下载 Feed");

            var job = new FeedJob();
            var snap = job.DoTask(feed, false);
            reporter.Report("Feed 下载完成");

            reporter.Report("正在提取Feed地址");
            var j = new FeedExtractJob();
            var urls = j.ExtractAddress(snap);
            reporter.Report("Feed地址提取完成");

            foreach (var url in urls)
            {
                reporter.Report("正在提取地址 " + url);
                var r = ContentQueue.Instance.Extract(url);
                results.AddRange(r);
            }

            reporter.Report("计算完成");

            return results;
        }
    }
}