using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Node.Feed.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            Logger.GetLogger(baseUrl).Info(baseUrl + " feed scheduler starting");

            FeedScheduler.baseUrl = baseUrl;
            FeedScheduler.proxyUrl = proxyUrl;
            FeedScheduler.feedNode = feedNode;

            scheduler = await factory.GetScheduler();
            await scheduler.Start();

            Logger.GetLogger(baseUrl).Info(baseUrl + " feed scheduler started");

            await SyncFeed();

            AddExtractJob();
        }

        private static async void AddExtractJob()
        {
            Logger.GetLogger(baseUrl).Info(baseUrl + " add extract job");

            var job = JobBuilder.Create<FeedExtractJob>()
                .WithIdentity("extract", "extract")
                .Build();

            job.JobDataMap.Add("baseUrl", baseUrl);

            var trigger = TriggerBuilder.Create().WithCronSchedule("0 0/1 * * * ?")
                .WithIdentity("extract")
                .Build();
            await scheduler.ScheduleJob(job, trigger);
        }

        public static async void AddJob(string jobKey, string[] cornExpressions, Dictionary<string, object> dic = null)
        {
            scheduler = await factory.GetScheduler();

            var exists = await scheduler.CheckExists(new JobKey(jobKey));

            if (!exists)
            {
                var job = JobBuilder.Create<FeedJob>()
                    .WithIdentity(jobKey,"feed")
                    .Build();

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
                        var trigger = TriggerBuilder.Create().WithCronSchedule(cornExpression).WithIdentity(jobKey).Build();
                        await scheduler.ScheduleJob(job, trigger);

                        Logger.GetLogger(baseUrl).Info("add job with feed id " + jobKey);
                    }
                    catch (Exception ex)
                    {
                        Logger.GetLogger(baseUrl).Error("job with feed id " + jobKey + " say: " + ex.Message);
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
            var exists = scheduler.CheckExists(job);
            exists.Wait();

            if (exists.Result)
            {
                var t = scheduler.DeleteJob(job);

                t.Wait();
                Logger.GetLogger(baseUrl).Info("delete job with feed id " + jobKey + " " + t.Result);
                return t.Result;
            }
            return false;
        }

        public static async void Stop()
        {
            await scheduler.Shutdown(false);

            Logger.GetLogger(baseUrl).Info("feed scheduler stoped");
        }

        public static Task SyncFeed()
        {
            return Task.Run(() =>
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

                            Logger.GetLogger(baseUrl).Info("sync feed and add feed jobs:" + feeds.Count);

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

                        Logger.GetLogger(baseUrl).Info("sync feed and add feed jobs:" + feeds.Count);
                    }
                }
                catch (Exception ex)
                {
                    Logger.GetLogger(baseUrl).Error("sync feed error " + ex.Message);
                }
            });
        }

        public static void OnReceive(BroadcastEvent @event)
        {
            Logger.GetLogger(baseUrl).Info("receive feed proxy broadcast");

            if (@event.Event == BroadcastEventEnum.UPDATE)
            {
                var feed = @event.Args as FeedModel;

                DeleteJob(feed.Id.ToString());

                if (feed.Status == Status.ON)
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