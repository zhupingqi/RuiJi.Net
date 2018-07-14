using Quartz;
using Quartz.Impl;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Node.Feed.Db;
using System;
using System.Collections.Generic;
using RuiJi.Net.Core.Utils.Logging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Net.Core.RTS;
using RuiJi.Net.Core.Crawler;
using Newtonsoft.Json;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class FeedScheduler
    {
        private static IScheduler scheduler;
        private static StdSchedulerFactory factory;

        private static string baseUrl;
        private static string proxyUrl;
        private static FeedNode feedNode;

        private static int[] feedPages;

        static FeedScheduler()
        {
            factory = new StdSchedulerFactory();
        }

        public static async void Start(string baseUrl, string proxyUrl, FeedNode feedNode)
        {
            FeedScheduler.baseUrl = baseUrl;
            FeedScheduler.proxyUrl = proxyUrl;
            FeedScheduler.feedNode = feedNode;

            scheduler = await factory.GetScheduler();
            await scheduler.Start();

            SyncFeed();
        }

        public static async void AddJob(string jobKey, string[] cornExpressions, Dictionary<string, object> dic = null)
        {
            scheduler = await factory.GetScheduler();

            var exists = await scheduler.CheckExists(new JobKey(jobKey));

            if (!exists)
            {
                IJobDetail job = JobBuilder.Create<FeedJob>().WithIdentity(jobKey).Build();

                job.JobDataMap.Add("proxyUrl", proxyUrl);
                job.JobDataMap.Add("baseUrl", baseUrl);
                job.JobDataMap.Add("node", feedNode);

                if (dic != null)
                {
                    foreach (var key in dic.Keys)
                    {
                        job.JobDataMap.Add(key, dic[key]);
                    }
                }

                foreach (var cornExpression in cornExpressions)
                {
                    try
                    {
                        ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(cornExpression).WithIdentity(jobKey).Build();
                        await scheduler.ScheduleJob(job, trigger);
                    }
                    catch (Exception ex)
                    {
                        Logger.GetLogger(baseUrl).Error("job[" + jobKey + "] say: " + ex.Message);
                    }
                }
            }
        }

        public static void AddJob(string jobKey, string cornExpression, Dictionary<string, object> dic = null)
        {
            if (string.IsNullOrEmpty(cornExpression))
            {
                return;
            }

            cornExpression = cornExpression.Replace("\r\n", "\n");

            AddJob(jobKey, cornExpression.Split('\n'), dic);
        }

        public static void AddJob(FeedModel feed)
        {
            var feedRequest = FeedModel.ToFeedRequest(feed);
            var dic = new Dictionary<string, object>();
            dic.Add("request", feedRequest);

            AddJob(feed.Id.ToString(), feed.Scheduling, dic);
        }

        public static bool DeleteJob(string jobKey)
        {
            var job = new JobKey(jobKey);

            var t = scheduler.DeleteJob(job);

            t.Wait();

            return t.Result;
        }

        public static async void Stop()
        {
            await scheduler.Shutdown(false);
        }

        public static async void SyncFeed()
        {
            await Task.Run(() =>
            {
                try
                {
                    scheduler.Clear();

                    if (NodeConfigurationSection.Standalone)
                    {
                        var page = 1;

                        var paging = new Paging();
                        paging.CurrentPage = page;
                        paging.PageSize = 5000;

                        var feeds = FeedLiteDb.GetAvailableFeeds(paging);
                        while (feeds.Count != 0)
                        {
                            foreach (var feed in feeds)
                            {
                                AddJob(feed);
                            }

                            Logger.GetLogger(baseUrl).Info("add feed jobs:" + feeds.Count);

                            paging.CurrentPage = ++page;
                            feeds = FeedLiteDb.GetAvailableFeeds(paging);
                        }
                    }
                    else
                    {
                        var data = feedNode.GetData("/config/feed/" + baseUrl);
                        var config = JsonConvert.DeserializeObject<NodeConfig>(data.Data);
                        if (config.Pages == null || config.Pages.Length == 0)
                            return;
                        feedPages = config.Pages;

                        var feedsResponse = NodeVisitor.Feeder.GetFeedJobs(proxyUrl, string.Join(",", config.Pages));
                        if (string.IsNullOrEmpty(feedsResponse))
                            throw new Exception("feedproxy can't connect");

                        var feeds = JsonConvert.DeserializeObject<List<FeedModel>>(feedsResponse);

                        foreach (var feed in feeds)
                        {
                            AddJob(feed);
                        }

                        Logger.GetLogger(baseUrl).Info("add feed jobs:" + feeds.Count);
                    }
                }
                catch (Exception ex)
                {
                    Logger.GetLogger(baseUrl).Error("get feed error " + ex.Message);
                }
            });
        }

        public static void OnReceive(BroadcastEvent @event)
        {
            if (@event.Event == BroadcastEventEnum.UPDATE)
            {
                var feed = @event.Args as FeedModel;
                if (feed.Status == Status.OFF)
                    DeleteJob(feed.Id.ToString());
                else
                {
                    if (NodeConfigurationSection.Standalone)
                    {
                        AddJob(feed);
                    }
                    else
                    {
                        var p = Convert.ToInt32(Math.Floor((feed.Id - 1) / 50.0)) + 1;
                        if (feedPages.Contains(p))
                        {
                            AddJob(feed);
                        }
                    }
                }
            }
            else
            {
                var ids = @event.Args as int[];

                foreach (var id in ids)
                {
                    DeleteJob(id.ToString());
                }
            }
        }
    }
}
