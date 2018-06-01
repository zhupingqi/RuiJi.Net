using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Feed.LTS
{
    public class FeedScheduler
    {
        private static IScheduler scheduler;
        private static StdSchedulerFactory factory;

        static FeedScheduler()
        {
            factory = new StdSchedulerFactory();
        }

        public static async void Start(string proxyUrl, FeedNode feedNode)
        {
            scheduler = await factory.GetScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<FeedJob>().Build();
            job.JobDataMap.Add("proxyUrl", proxyUrl);
            job.JobDataMap.Add("node", feedNode);

            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule("0 0/1 * * * ?").Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public static async void Stop()
        {
            await scheduler.Shutdown(false);
        }
    }
}
