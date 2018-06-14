using Amib.Threading;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Node.Db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class FeedJob : IJob
    {
        public static bool IsRunning = false;

        private static string baseDir;

        static FeedJob()
        {
            baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(baseDir + @"snapshot"))
            {
                Directory.CreateDirectory(baseDir + @"snapshot");
            }

            if (!Directory.Exists(baseDir + @"delay"))
            {
                Directory.CreateDirectory(baseDir + @"delay");
            }
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (!IsRunning)
            {
                IsRunning = true;

                var task = Task.Factory.StartNew(() =>
                {
                    var proxyUrl = context.JobDetail.JobDataMap.Get("proxyUrl").ToString();
                    var node = context.JobDetail.JobDataMap.Get("node") as FeedNode;

                    var feeds = GetFeedJobs(proxyUrl, node);
                    var compile = new CompileFeedAddress();

                    var stpStartInfo = new STPStartInfo
                    {
                        IdleTimeout = 3000,
                        MaxWorkerThreads = 8,
                        MinWorkerThreads = 0
                    };

                    var pool = new SmartThreadPool(stpStartInfo);
                    var waits = new List<IWorkItemResult>();

                    foreach (var feed in feeds)
                    {
                        var addrs = compile.Compile(feed.Address);
                        foreach (var addr in addrs)
                        {
                            feed.Address = addr;

                            var item = pool.QueueWorkItem((u) =>
                            {
                                DoTask(u, true);
                            }, feed);

                            waits.Add(item);
                        }
                    }

                    SmartThreadPool.WaitAll(waits.ToArray());
                    pool.Shutdown(true, 1000);
                    pool.Dispose();
                    pool = null;
                    waits.Clear();
                });

                await task;

                IsRunning = false;
            }
        }

        private List<FeedModel> GetFeedJobs(string proxyUrl,FeedNode node)
        {
            try
            {
                var d = node.GetData("/config/feed/" + node.BaseUrl);
                var config = JsonConvert.DeserializeObject<NodeConfig>(d.Data);
                var pages = config.Pages == null ? "" : string.Join(",", config.Pages);

                var client = new RestClient("http://" + proxyUrl);
                var restRequest = new RestRequest("api/feed/job?pages=" + pages);
                restRequest.Method = Method.GET;

                var restResponse = client.Execute(restRequest);

                var feeds = JsonConvert.DeserializeObject<List<FeedModel>>(restResponse.Content);

                return feeds;
            }
            catch (Exception ex)
            {
                return new List<FeedModel>();
            }
        }

        public FeedSnapshot DoTask(FeedModel feed,bool persistence = false)
        {
            try
            {
                var request = new Request(feed.Address);
                if (feed.Headers != null)
                    request.Headers = feed.Headers;
                request.Headers.Add(new WebHeader("Referer", request.Uri.AbsoluteUri));
                //request.Headers.Add(new WebHeader("Referer", "https://www.baidu.com/link?url=GEyGoQq22aGfAidB32foRlg2BWxPgspy0KlenTlTNoucDZr0sdVFfXrwtO6xs_Xe&wd=&eqid=9c546d780000aeba000000025b2248a2"));
                request.Method = feed.Method;
                if (feed.Method == "POST" && !string.IsNullOrEmpty(feed.PostParam))
                    request.PostParam = feed.PostParam;

                var response = NodeVisitor.Crawler.Request(request);

                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    var content = Convert(response.Data.ToString(), Encoding.GetEncoding(response.Charset), Encoding.UTF8);

                    var snap = new FeedSnapshot
                    {
                        Url = feed.Address,
                        Content = content,
                        Type = feed.Type,
                        BlockExpression = feed.BlockExpression,
                        RuiJiExpression = feed.RuiJiExpression
                    };

                    if (persistence)
                    {
                        var json = JsonConvert.SerializeObject(snap, Formatting.Indented);

                        var fileName = baseDir + @"snapshot\" + feed.Id + "_" + DateTime.Now.Ticks + ".json";
                        if (feed.Delay > 0)
                        {
                            fileName = baseDir + @"delay\" + feed.Id + "_" + DateTime.Now.AddMinutes(feed.Delay).Ticks + ".json";
                        }

                        File.WriteAllText(fileName, json, Encoding.UTF8);
                    }

                    return snap;
                }
            }
            catch (Exception ex)
            {
                
            }

            return null;
        }

        private string Convert(string input, Encoding source, Encoding target)
        {
            var bytes = source.GetBytes(input);
            var dst = Encoding.Convert(source, target, bytes);
            return target.GetString(dst);
        }
    }
}