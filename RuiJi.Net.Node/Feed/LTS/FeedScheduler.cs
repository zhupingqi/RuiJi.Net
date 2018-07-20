using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public static Dictionary<string, FeedScheduler> Schedulers { get; private set; }

        private string baseUrl;
        private FeedNode feedNode;
        private string jobGroup { get { return "feed" + baseUrl; } }

        private int[] feedPages;

        static FeedScheduler()
        {
            Schedulers = new Dictionary<string, FeedScheduler>();

            factory = new StdSchedulerFactory();
            var factoryResult = factory.GetScheduler();
            factoryResult.Wait();

            scheduler = factoryResult.Result;
            scheduler.Start();
        }

        public async void Start(string baseUrl, FeedNode feedNode)
        {
            if (Schedulers.ContainsKey(baseUrl))
                return;

            Logger.GetLogger(baseUrl).Info(baseUrl + " feed scheduler starting");

            this.baseUrl = baseUrl;
            this.feedNode = feedNode;
            Schedulers.Add(baseUrl, this);

            Logger.GetLogger(baseUrl).Info(baseUrl + " feed scheduler started");

            await SyncFeed();

            AddExtractJob();
        }

        private async void AddExtractJob()
        {
            Logger.GetLogger(baseUrl).Info(baseUrl + " add extract job");

            var job = JobBuilder.Create<FeedExtractJob>()
                .WithIdentity("extract_" + baseUrl, "extract")
                .Build();

            job.JobDataMap.Add("baseUrl", baseUrl);

            var trigger = TriggerBuilder.Create().WithCronSchedule("0 0/1 * * * ?")
                .WithIdentity("extract_" + baseUrl)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public async void AddJob(string jobKey, string[] cornExpressions, Dictionary<string, object> dic = null)
        {
            scheduler = await factory.GetScheduler();

            var exists = await scheduler.CheckExists(new JobKey(jobKey, jobGroup));

            if (!exists)
            {
                var job = JobBuilder.Create<FeedJob>()
                    .WithIdentity(jobKey, jobGroup)
                    .Build();

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

                        Logger.GetLogger(baseUrl).Info(baseUrl + " add job with feed id " + jobKey);
                    }
                    catch (Exception ex)
                    {
                        Logger.GetLogger(baseUrl).Error(baseUrl + " job with feed id " + jobKey + " say: " + ex.Message);
                    }
                }
            }
        }

        public void AddJob(string jobKey, string cornExpression, Dictionary<string, object> dic = null)
        {
            if (string.IsNullOrEmpty(cornExpression))
            {
                return;
            }

            cornExpression = cornExpression.Replace("\r\n", "\n");

            AddJob(jobKey, cornExpression.Split('\n'), dic);
        }

        public void AddJob(FeedModel feed)
        {
            var feedRequest = FeedModel.ToFeedRequest(feed);
            var dic = new Dictionary<string, object>();
            dic.Add("request", feedRequest);

            AddJob(feed.Id.ToString(), feed.Scheduling, dic);
        }

        public bool DeleteJob(string jobKey)
        {
            var job = new JobKey(jobKey, jobGroup);
            var exists = scheduler.CheckExists(job);
            exists.Wait();

            if (exists.Result)
            {
                var t = scheduler.DeleteJob(job);

                t.Wait();
                Logger.GetLogger(baseUrl).Info(baseUrl + " delete job with feed id " + jobKey + " " + t.Result);
                return t.Result;
            }
            return false;
        }

        public async void Stop()
        {
            Schedulers.Remove(baseUrl);

            await scheduler.Shutdown(false);

            Logger.GetLogger(baseUrl).Info(baseUrl + " feed scheduler stoped");
        }

        public Task SyncFeed()
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

                            Logger.GetLogger(baseUrl).Info(baseUrl + " sync feed and add feed jobs:" + feeds.Count);

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

                        var feedsResponse = NodeVisitor.Feeder.GetFeedJobs(string.Join(",", config.Pages));
                        if (string.IsNullOrEmpty(feedsResponse))
                            throw new Exception("feedproxy can't connect");

                        var feeds = JsonConvert.DeserializeObject<List<FeedModel>>(feedsResponse);

                        foreach (var feed in feeds)
                        {
                            AddJob(feed);
                        }

                        Logger.GetLogger(baseUrl).Info(baseUrl + " sync feed and add feed jobs:" + feeds.Count);
                    }
                }
                catch (Exception ex)
                {
                    Logger.GetLogger(baseUrl).Error(baseUrl + " sync feed error " + ex.Message);
                }
            });
        }

        public void OnReceive(BroadcastEvent @event)
        {
            Logger.GetLogger(baseUrl).Info(baseUrl + " receive feed proxy broadcast");

            if (@event.Event == BroadcastEventEnum.UPDATE)
            {

                var feed = ((JObject)@event.Args).ToObject<FeedModel>();

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

        public static FeedScheduler GetSecheduler(string baseUrl)
        {
            if (!Schedulers.ContainsKey(baseUrl))
            {
                throw new Exception("feed scheduler is no instantiation!");
            }
            return Schedulers[baseUrl];
        }
    }
}