using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Core.Utils.Tasks;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Db;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
        public object Rules(int offset, int limit)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            return new
            {
                rows = RuleLiteDb.GetModels(paging),
                total = paging.Count
            };
        }

        [HttpGet]
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
        public object UrlRule(string url, bool useBlock = false)
        {
            if (useBlock)
                return RuleLiteDb.Match(url).Select(m => new ExtractFeatureBlock(JsonConvert.DeserializeObject<ExtractBlock>(m.BlockExpression), m.Feature)).ToList();
            else
            {
                return RuleLiteDb.Match(url).Select(m => new ExtractFeatureBlock(RuiJiExpression.ParserBlock(m.RuiJiExpression), m.Feature)).ToList();
            }
        }

        [HttpPost]
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
        public void UpdateRule(RuleModel rule)
        {
            RuleLiteDb.AddOrUpdate(rule);
        }

        [HttpGet]
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
        public object GetRule(int id)
        {
            var feed = RuleLiteDb.Get(id);

            return feed;
        }

        [HttpGet]
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
        public bool RemoveRule(string ids)
        {
            var removes = ids.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            return RuleLiteDb.Remove(removes);
        }

        [HttpPost]
        public object TestRule(RuleModel rule)
        {
            var c = new Crawler();
            var response = c.Request(rule.Url, rule.Method);
            if (response != null && response.Data != null)
            {
                var content = response.Data.ToString();
                var block = RuiJiExpression.ParserBlock(rule.RuiJiExpression);
                var r = new ExtractRequest();
                r.Content = content;
                r.Blocks = new List<ExtractFeatureBlock> {
                    new ExtractFeatureBlock {
                        Block = block,
                        Feature = rule.Feature
                    }
                };

                var results = Extracter.Extract(r);

                var result = results.OrderByDescending(m => m.Metas.Count).FirstOrDefault();
                result.Content = null;

                return result;
            }

            return new { };
        }
        #endregion

        #region Feed
        [HttpGet]
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
        public object Feeds(int offset, int limit)
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

        [HttpGet]
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
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
        public string GetFeedPage()
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var d = node.GetData("/config/feed/" + Request.RequestUri.Authority).Data;
            var config = JsonConvert.DeserializeObject<NodeConfig>(d);

            if (config.Pages == null)
                config.Pages = new int[0];

            return string.Join(",", config.Pages);
        }

        [HttpPost]
        public void SetFeedPage([FromBody]string pages)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var path = "/config/feed/" + Request.RequestUri.Authority;

            var data = node.GetData("/config/feed/" + Request.RequestUri.Authority);
            var config = JsonConvert.DeserializeObject<NodeConfig>(data.Data);
            config.Pages = pages.Split(',').Select(m => Convert.ToInt32(m)).ToArray();

            node.SetData(path, JsonConvert.SerializeObject(config));
        }

        [HttpPost]
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
        public void UpdateFeed([FromBody]FeedModel feed)
        {
            FeedLiteDb.AddOrUpdate(feed);
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
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
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
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
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
                    feedId = m.Id,
                    url = m.Url,
                    metas = m.Metas.Select(n => new
                    {
                        name = n.Key,
                        content = n.Value
                    })
                }),
                total = paging.Count
            };
        }
        #endregion

        #region Test
        [HttpPost]
        public object TestFeed(FeedModel feed)
        {
            try
            {
                var compile = new CompileFeedAddress();
                var addrs = compile.Compile(feed.Address);
                var results = new List<ExtractResult>();

                foreach (var addr in addrs)
                {
                    feed.Address = addr;
                    var job = new FeedJob();
                    var snap = job.DoTask(feed, false);

                    var block = RuiJiExpression.ParserBlock(feed.RuiJiExpression);

                    var result = RuiJiExtracter.Extract(snap.Content, block);
                    CrawlTaskFunc.ClearContent(result);

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

        #region 节点函数
        [HttpGet]
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
        public object Funcs(int offset, int limit)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            var list = FuncLiteDb.GetModels(paging);

            return new
            {
                rows = list,
                total = list.Count
            };
        }

        [HttpGet]
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
        public object GetFunc(int id)
        {
            return FuncLiteDb.Get(id);
        }

        [HttpPost]
        public object FuncTest(FuncModel func)
        {
            var code = "{# " + func.Sample + " #}";
            var test = new ComplieFuncTest(func.Code);
            return test.Compile(code);
        }

        [HttpPost]
        [NodeRoute(Target = NodeProxyTypeEnum.Feed)]
        public object UpdateFunc(FuncModel func)
        {
            if (func.Name == "now" || func.Name == "ticks")
                return false;

            var f = FuncLiteDb.Get(func.Name);
            if (f != null && f.Id == 0)
                return false;

            FuncLiteDb.AddOrUpdate(func);
            return true;
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

            var compile = new CompileFeedAddress();
            var addrs = compile.Compile(feed.Address);

            foreach (var addr in addrs)
            {
                feed.Address = addr;

                var job = new FeedJob();
                var snap = job.DoTask(feed, false);
                reporter.Report("Feed 下载完成");

                var block = RuiJiExpression.ParserBlock(feed.RuiJiExpression);

                var feedResult = RuiJiExtracter.Extract(snap.Content, block);
                results.Add(feedResult);

                reporter.Report("正在提取Feed地址");
                var j = new FeedExtractJob();
                var urls = j.ExtractAddress(snap);
                reporter.Report("Feed地址提取完成");

                if (!string.IsNullOrEmpty(snap.RuiJiExpression))
                {
                    var visitor = new Visitor();

                    foreach (var url in urls)
                    {
                        reporter.Report("正在提取地址 " + url);
                        var result = visitor.Extract(url);

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

    public class ComplieFuncTest : CompileUrl
    {
        private string code;

        public ComplieFuncTest(string code)
        {
            this.code = code;
        }

        public override string FormatCode(CompileExtract extract)
        {
            var formatCode = string.Format(code, extract.Args);

            return formatCode;
        }
    }
}