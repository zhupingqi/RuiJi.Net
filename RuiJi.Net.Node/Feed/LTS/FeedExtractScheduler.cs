using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class FeedExtractScheduler
    {
        private static IScheduler scheduler;
        private static StdSchedulerFactory factory;

        static FeedExtractScheduler()
        {
            factory = new StdSchedulerFactory();
        }

        public static async void Start(string baseUrl)
        {
            scheduler = await factory.GetScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<FeedExtractJob>().Build();
            job.JobDataMap.Add("baseUrl", baseUrl);

            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule("0 0/1 * * * ?").Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public static async void Stop()
        {
            await scheduler.Shutdown(false);
        }
    }
}
