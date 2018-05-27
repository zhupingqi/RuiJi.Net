using Amib.Threading;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using RuiJi.Core.Crawler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Feed.LTS
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
                        var item = pool.QueueWorkItem((u) =>
                        {
                            DoTask(u);
                        }, feed);

                        waits.Add(item);
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

        public void DoTask(FeedModel feed)
        {
            try
            {
                var request = new Request(feed.Url);
                request.Headers = feed.Headers;
                request.Headers.Add(new WebHeader("Referer", request.Uri.AbsoluteUri));
                request.Method = feed.Method;
                if (feed.Method == "POST" && !string.IsNullOrEmpty(feed.PostParam))
                    request.PostParam = feed.PostParam;

                var response = new RuiJi.Net.Crawler().Request(request);

                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    var fileName = baseDir + @"snapshot\" + feed.Id + "_" + DateTime.Now.Ticks + ".json";
                    if (feed.Delay > 0)
                    {
                        fileName = baseDir + @"delay\" + feed.Id + "_" + DateTime.Now.AddMinutes(feed.Delay).Ticks + ".json";
                    }

                    var content = Convert(response.Data.ToString(), Encoding.GetEncoding(response.Charset), Encoding.UTF8);

                    var json = JsonConvert.SerializeObject(new FeedSnapshot
                    {
                        Url = feed.Url,
                        Content = content,
                        Type = feed.Type,
                        ExtractBlock = feed.ExtractBlock
                    }, Formatting.Indented);

                    File.WriteAllText(fileName, json, Encoding.UTF8);

                    //SeedCache.Instance.SetLastVisitTime(url, DateTime.Now);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private string Convert(string input, Encoding source, Encoding target)
        {
            var bytes = source.GetBytes(input);
            var dst = Encoding.Convert(source, target, bytes);
            return target.GetString(dst);
        }
    }
}