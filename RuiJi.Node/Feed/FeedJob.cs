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

namespace RuiJi.Node.Feed
{
    public class FeedJob : IJob
    {
        public static bool IsRunning = false;
        
        internal static string ProxyUrl { get; set; }

        private static string baseDir;

        static FeedJob()
        {
            baseDir = AppDomain.CurrentDomain.BaseDirectory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (!IsRunning)
            {
                IsRunning = true;

                await Run();

                IsRunning = false;
            }
        }

        private Task Run()
        {
            var task = Task.Factory.StartNew(() => {
                var client = new RestClient("http://" + ProxyUrl);
                var restRequest = new RestRequest("api/feed/job");
                restRequest.Method = Method.GET;

                var restResponse = client.Execute(restRequest);

                try
                {
                    var feeds = JsonConvert.DeserializeObject<List<FeedModel>>(restResponse.Content);

                    if (feeds.Count > 0)
                    {
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
                    }
                }
                catch(Exception ex)
                {

                }
            });

            task.ConfigureAwait(false);

            return task;
        }

        public void DoTask(FeedModel feed)
        {
            var url = feed.Url;
            var feedId = feed.Id;
            var delay = feed.Delay;
            var type = feed.Type;

            try
            {
                var request = new Request(url);
                request.Headers.Add(new WebHeader("Referer", request.Uri.AbsoluteUri));
                request.Method = feed.Method;
                if (feed.Method == "POST" && !string.IsNullOrEmpty(feed.PostParam))
                    request.PostParam = feed.PostParam;
                request.Headers = feed.Headers;       

                var response = new RuiJi.Net.Crawler().Request(request);

                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    var fileName = baseDir + @"snapshot\" + feedId + "_" + DateTime.Now.Ticks + ".json";
                    if (delay > 0)
                    {
                        fileName = baseDir + @"delay\" + feedId + "_" + DateTime.Now.AddMinutes(delay).Ticks + ".json";
                    }

                    var content = Convert(response.Data.ToString(), Encoding.GetEncoding(response.Charset), Encoding.UTF8);

                    var json = JsonConvert.SerializeObject(new
                    {
                        url = url,
                        content = content,
                        type = type
                    }, Formatting.Indented);

                    File.WriteAllText(fileName, json, Encoding.UTF8);

                    //SeedCache.Instance.SetLastVisitTime(url, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public string Convert(string input, Encoding source, Encoding target)
        {
            var bytes = source.GetBytes(input);
            var dst = Encoding.Convert(source, target, bytes);
            return target.GetString(dst);
        }
    }
}
