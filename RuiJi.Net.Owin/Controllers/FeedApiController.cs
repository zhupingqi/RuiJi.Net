using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Core.Utils.Tasks;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Db;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Storage;
using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Net.Owin.Controllers
{
    public class FeedApiController : ApiController
    {
        #region Rule
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object Rules(int offset, int limit, string key, string type, string status)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            return new
            {
                rows = RuleLiteDb.GetModels(paging, key, type, status),
                total = paging.Count
            };
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object UrlRule(string url, bool useBlock = false)
        {
            if (useBlock)
                return RuleLiteDb.Match(url).Select(m => new ExtractFeatureBlock(JsonConvert.DeserializeObject<ExtractBlock>(m.BlockExpression), m.Feature)).ToList();
            else
            {
                return RuleLiteDb.Match(url).Select(m => new ExtractFeatureBlock(RuiJiExtractBlockParser.ParserBlock(m.RuiJiExpression), m.Feature)).ToList();
            }
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public void UpdateRule(RuleModel rule)
        {
            RuleLiteDb.AddOrUpdate(rule);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object GetRule(int id)
        {
            var feed = RuleLiteDb.Get(id);

            return feed;
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public bool RuleStatusChange(string ids, string status)
        {
            var changeIds = ids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
            var statusEnum = (Status)Enum.Parse(typeof(Status), status.ToUpper());

            return RuleLiteDb.StatusChange(changeIds, statusEnum);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public bool RemoveRule(string ids)
        {
            var removes = ids.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            return RuleLiteDb.Remove(removes);
        }
        #endregion

        #region Feed
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
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

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object FeedJob(string pages)
        {
            try
            {
                var ps = pages.Split(',').Select(m => Convert.ToInt32(m)).ToArray();
                var feeds = FeedLiteDb.GetFeedModels(ps, 50);
                feeds.RemoveAll(m => m.Status == Status.OFF);

                return feeds;
            }
            catch { }

            return new { };
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEED, RouteArgumentName = "baseUrl")]
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
        public void SetFeedPage([FromBody]string pages, [FromUri]string baseUrl)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var path = "/config/feed/" + Request.RequestUri.Authority;

            var data = node.GetData("/config/feed/" + Request.RequestUri.Authority);
            var config = JsonConvert.DeserializeObject<NodeConfig>(data.Data);
            config.Pages = pages.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            node.SetData(path, JsonConvert.SerializeObject(config));
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public void UpdateFeed([FromBody]FeedModel feed)
        {
            FeedLiteDb.AddOrUpdate(feed);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public bool FeedStatusChange(string ids, string status)
        {
            var changeIds = ids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
            var statusEnum = (Status)Enum.Parse(typeof(Status), status.ToUpper());

            return FeedLiteDb.StatusChange(changeIds, statusEnum);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public bool RemoveFeed(string ids)
        {
            var removes = ids.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            return FeedLiteDb.Remove(removes);
        }

        [HttpGet]
        public object GetFeed(int id)
        {
            var feed = FeedLiteDb.GetFeed(id);

            return feed;
        }
        #endregion

        #region 存储抓取结果
        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public bool SaveContent(ContentModel content, string shard = "")
        {
            try
            {
                if (string.IsNullOrEmpty(shard))
                    shard = DateTime.Now.ToString("yyyyMM");

                var storage = new LiteDbStorage(@"LiteDb/Content/" + shard + ".db", "contents");
                storage.Insert(content);

            }
            catch
            {
                return false;
            }

            return true;
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object GetContent(int offset, int limit, string shard = "", int feedId = 0)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            if (string.IsNullOrEmpty(shard))
                shard = DateTime.Now.ToString("yyyyMM");

            return new
            {
                rows = ContentLiteDb.GetModels(paging, shard, feedId).Select(m => new
                {
                    id = m.Id,
                    feedId = m.FeedId,
                    url = m.Url,
                    cdate = m.CDate,
                    metas = m.Metas.Select(n => new
                    {
                        name = n.Key,
                        content = n.Value.ToString().Length > 50 ? n.Value.ToString().Substring(0, 50) : n.Value.ToString()
                    })
                }),
                total = paging.Count
            };
        }
        #endregion

        #region Test

        [HttpPost]
        public object TestRule(RuleModel rule, [FromUri]bool debug = false)
        {
            var request = new Request(rule.Url);
            request.Method = rule.Method;
            request.RunJS = (rule.RunJS == Status.ON);

            var response = Crawler.Request(request);
            if (response != null && response.Data != null)
            {
                var content = response.Data.ToString();
                var block = RuiJiExtractBlockParser.ParserBlock(rule.RuiJiExpression);
                var r = new ExtractRequest();
                r.Content = content;

                r.Blocks = new List<ExtractFeatureBlock> {
                    new ExtractFeatureBlock (block,rule.Feature)
                };

                var results = Extractor.Extract(r);

                var result = results.OrderByDescending(m => m.Metas.Count).FirstOrDefault();

                if (result.Paging != null && result.Paging.Count > 0 && result.Metas != null && result.Metas.ContainsKey("content"))
                {
                    result = PagingExtractor.MergeContent(new Uri(rule.Url), result, block);
                }

                if (!debug)
                    CrawlTaskFunc.ClearContent(result);

                return result;
            }

            return new { };
        }

        [HttpPost]
        public object TestFeed(FeedModel feed, [FromUri]bool down, [FromUri]bool debug = false)
        {
            try
            {
                var compile = new UrlCompile();
                var addrs = compile.GetResult(feed.Address);
                var results = new List<ExtractResult>();

                foreach (var addr in addrs)
                {
                    feed.Address = addr.ToString();
                    var job = new FeedJob();
                    var snap = job.DoTask(feed, false);

                    if (string.IsNullOrEmpty(feed.RuiJiExpression))
                    {
                        results.Add(new ExtractResult());
                        continue;
                    }

                    var block = RuiJiExtractBlockParser.ParserBlock(feed.RuiJiExpression);

                    var result = RuiJiExtractor.Extract(snap.Content, block);

                    if (!debug)
                        CrawlTaskFunc.ClearContent(result);

                    if (down)
                    {
                        var s = new FileStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "www", "download"));

                        var files = result.Content.ToString().Replace("\r\n", "\n").Split('\n');
                        foreach (var file in files)
                        {
                            if (!string.IsNullOrEmpty(file) && Uri.IsWellFormedUriString(file, UriKind.Absolute))
                            {
                                var res = Crawler.Request(file);
                                var c = new DownloadContentModel();
                                c.Url = file.Trim();
                                c.IsRaw = res.IsRaw;
                                c.Data = res.Data;

                                s.Insert(c);
                            }
                        }
                    }

                    results.Add(result);
                }

                return results;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        [HttpGet]
        public object RunCrawl([FromUri]CrawlTaskModel crawlTask)
        {
            ParallelTask task;

            if (crawlTask.TaskId == 0)
            {
                task = ParallelTaskManager.StartNew<CrawlTaskFunc>(crawlTask);
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
        #endregion
    }

    public class CrawlTaskModel
    {
        [JsonProperty("feedId")]
        public int FeedId { get; set; }

        [JsonProperty("taskId")]
        public int TaskId { get; set; }

        [JsonProperty("content")]
        public bool IncludeContent { get; set; }
    }

    public class CrawlTaskFunc : IParallelTaskFunc
    {
        public object Run(object t, ParallelTask task)
        {
            var model = t as CrawlTaskModel;

            var results = new List<object>();
            var reporter = task.Progress as IProgress<string>;

            reporter.Report("正在读取Feed记录");
            var feed = FeedLiteDb.GetFeed(model.FeedId);

            reporter.Report("正在下载 Feed");

            var compile = new UrlCompile();
            var addrs = compile.GetResult(feed.Address);

            foreach (var addr in addrs)
            {
                feed.Address = addr.ToString();

                var job = new FeedJob();
                var snap = job.DoTask(feed, false);
                reporter.Report("Feed 下载完成");

                var block = RuiJiExtractBlockParser.ParserBlock(feed.RuiJiExpression);

                var feedResult = RuiJiExtractor.Extract(snap.Content, block);
                results.Add(feedResult);

                reporter.Report("正在提取Feed地址");
                var j = new FeedExtractJob();
                var urls = j.ExtractAddress(snap);
                reporter.Report("Feed地址提取完成");

                if (!string.IsNullOrEmpty(snap.RuiJiExpression))
                {
                    foreach (var url in urls)
                    {
                        reporter.Report("正在提取地址 " + url);
                        var result = Cooperater.GetResult(url);

                        if (result != null)
                        {
                            var cm = new ContentModel();
                            cm.Id = model.FeedId;
                            cm.Url = url;
                            cm.Metas = result.Metas;
                            cm.CDate = DateTime.Now;

                            results.Add(cm);
                        }
                    }
                }

                reporter.Report("计算完成");

                if (!model.IncludeContent)
                {
                    results.ForEach((m) =>
                    {
                        ClearContent(m);
                    });
                }
            }

            return results;
        }

        public static void ClearContent(object obj)
        {
            var result = obj as ExtractResult;
            if (result == null)
                return;

            if (result.Blocks != null || result.Metas != null || result.Tiles != null)
            {
                result.Content = null;
            }

            if (result.Tiles != null)
            {
                foreach (var tile in result.Tiles)
                {
                    if (tile.Metas != null)
                        tile.Content = null;
                }
            }

            if (result.Blocks != null && result.Blocks.Count > 0)
            {
                result.Blocks.ForEach((m) =>
                {
                    ClearContent(m);
                });
            }
        }
    }
}