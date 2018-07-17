using Quartz;
using Quartz.Impl;
using RuiJi.Net.Core.Utils.Logging;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class FeedExtractScheduler
    {
        private static IScheduler scheduler;
        private static StdSchedulerFactory factory;
        private static string baseUrl;

        static FeedExtractScheduler()
        {
            factory = new StdSchedulerFactory();
        }

        public static async void Start(string baseUrl)
        {
            FeedExtractScheduler.baseUrl = baseUrl;

            Logger.GetLogger(baseUrl).Info(baseUrl + " extract scheduler starting");

            scheduler = await factory.GetScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<FeedExtractJob>().Build();
            job.JobDataMap.Add("baseUrl", baseUrl);

            var trigger = TriggerBuilder.Create().WithCronSchedule("0 0/1 * * * ?").Build();
            await scheduler.ScheduleJob(job, trigger);

            Logger.GetLogger(baseUrl).Info(baseUrl + " extract scheduler started");
        }

        public static async void Stop()
        {
            await scheduler.Shutdown(false);

            Logger.GetLogger(baseUrl).Info("extract scheduler stoped");
        }
    }
}
