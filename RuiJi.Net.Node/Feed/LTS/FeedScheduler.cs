using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class FeedScheduler
    {
        private static IScheduler scheduler;
        private static StdSchedulerFactory factory;

        static FeedScheduler()
        {
            factory = new StdSchedulerFactory();
        }

        public static async void Start(string baseUrl,string proxyUrl, FeedNode feedNode)
        {
            scheduler = await factory.GetScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<FeedJob>().Build();
            job.JobDataMap.Add("proxyUrl", proxyUrl);
            job.JobDataMap.Add("baseUrl", baseUrl);
            job.JobDataMap.Add("node", feedNode);

            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule("0 0/5 * * * ?").Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public static async void AddJob(string jobKey, string[] cornExpressions, Dictionary<string, object> dic = null)
        {
            scheduler = await factory.GetScheduler();

            var exists = await scheduler.CheckExists(new JobKey(jobKey));

            if (!exists)
            {
                IJobDetail job = JobBuilder.Create<FeedJob>().WithIdentity(jobKey).Build();

                if (dic != null)
                {
                    foreach (var key in dic.Keys)
                    {
                        job.JobDataMap.Add(key, dic[key]);
                    }
                }

                foreach (var cornExpression in cornExpressions)
                {
                    ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(cornExpression).WithIdentity(jobKey).Build();
                    await scheduler.ScheduleJob(job, trigger);
                }
            }
        }

        public static void AddJob(string jobKey, string cornExpression, Dictionary<string, object> dic = null)
        {
            AddJob(jobKey, new string[] { cornExpression }, dic);
        }

        public static async void DeleteJob(string jobKey)
        {
            var job = new JobKey(jobKey);

            await scheduler.DeleteJob(job);
        }

        public static async void Stop()
        {
            await scheduler.Shutdown(false);
        }
    }
}
