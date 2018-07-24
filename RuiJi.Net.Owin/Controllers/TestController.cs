using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RuiJi.Net.Core.Compile;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Utils.Tasks;
using RuiJi.Net.Node.Feed.Db;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Storage;
using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RuiJi.Net.Owin.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        #region Test
        [HttpPost]
        [Route("rule")]
        public object TestRule([FromBody]RuleModel rule, bool debug = false)
        {
            var request = new Request(rule.Url);
            request.Method = rule.Method;
            request.RunJS = (rule.RunJS == Status.ON);

            var response = Crawler.Request(request);
            if (response != null && response.Data != null)
            {
                var content = response.Data.ToString();
                var block = RuiJiBlockParser.ParserBlock(rule.RuiJiExpression);
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
        [Route("feed")]
        public object TestFeed([FromBody]FeedModel feed, bool down, bool debug = false)
        {
            try
            {
                var compile = new Node.Compile.UrlCompile();
                var addrs = compile.GetResult(feed.Address);
                var results = new List<ExtractResult>();

                foreach (var addr in addrs)
                {
                    feed.Address = addr.ToString();
                    var job = new FeedJob();
                    var response = job.DoTask(feed);

                    if (string.IsNullOrEmpty(feed.RuiJiExpression))
                    {
                        results.Add(new ExtractResult());
                        continue;
                    }

                    var block = RuiJiBlockParser.ParserBlock(feed.RuiJiExpression);

                    var result = RuiJiExtractor.Extract(response.Data.ToString(), block);

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
        [Route("crawl")]
        public object RunCrawl(CrawlTaskModel crawlTask)
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

        [HttpPost]
        [Route("func")]
        public object FuncTest([FromBody]FuncModel func)
        {
            var code = "{# " + func.Sample + " #}";
            var test = new ComplieFuncTest(func.Code);
            return test.GetResult(code);
        }

        [HttpGet]
        [Route("proxy")]
        public object ProxyPing(int id)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {

                var crawler = new RuiJiCrawler();
                var request = new Request("https://www.baidu.com/");
                request.Timeout = 15000;

                var proxy = ProxyLiteDb.Get(id);
                request.Proxy = new RequestProxy(proxy.Ip, proxy.Port, proxy.UserName, proxy.Password);
                request.Proxy.Scheme = proxy.Type == ProxyTypeEnum.HTTP ? "http" : "https";

                var response = crawler.Request(request);

                watch.Stop();

                return new
                {
                    elspsed = watch.Elapsed.Milliseconds,
                    code = response.StatusCode,
                    msg = response.StatusCode.ToString()
                };
            }
            catch (Exception ex)
            {
                watch.Stop();

                return new
                {
                    elspsed = watch.Elapsed.Milliseconds,
                    code = -1,
                    msg = ex.Message
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

            var compile = new Node.Compile.UrlCompile();
            var addrs = compile.GetResult(feed.Address);

            foreach (var addr in addrs)
            {
                feed.Address = addr.ToString();

                var job = new FeedJob();
                var response = job.DoTask(feed);
                reporter.Report("Feed 下载完成");

                var block = RuiJiBlockParser.ParserBlock(feed.RuiJiExpression);

                var feedResult = RuiJiExtractor.Extract(response.Data.ToString(), block);
                results.Add(feedResult);

                var snap = new FeedSnapshot
                {
                    Url = feed.Address,
                    Content = response.Data.ToString(),
                    Type = feed.Type,
                    RuiJiExpression = feed.RuiJiExpression
                };

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

    public class ComplieFuncTest : Core.Compile.UrlCompile
    {
        private string code;

        public ComplieFuncTest(string code)
        {
            this.code = code;
        }

        protected override string FormatCode(UrlFunction result)
        {
            var formatCode = string.Format(code, result.Args);

            return formatCode;
        }
    }
}