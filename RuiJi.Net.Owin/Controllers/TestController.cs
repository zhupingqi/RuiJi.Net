using Microsoft.AspNetCore.Mvc;
using RuiJi.Net.Core.Code.Compiler;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Utils.Tasks;
using RuiJi.Net.Node.Feed.Db;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Owin.Models;
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
            if (request.RunJS)
                request.WaitDom = request.WaitDom;
            
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

                if (result != null && result.Paging != null && result.Paging.Count > 0 && result.Metas != null && result.Metas.ContainsKey("content"))
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
                //var compile = new Node.Compile.JSUrlCompile();
                var addrs = CodeCompilerManager.GetResult("url", feed.Address); //compile.GetResult(feed.Address);
                var results = new List<ExtractResult>();

                foreach (var addr in addrs)
                {
                    feed.Address = addr.ToString();
                    var job = new FeedJob();
                    var response = job.DoTask(feed);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return response.Data;
                    }
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
        public object RunCrawl([FromQuery]CrawlTaskModel crawlTask)
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
            var type = (func.Type == FuncType.URLFUNCTION) ? "url" : "proc";

            return CodeCompilerManager.Test(type, func.Sample, func.Code);
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
}