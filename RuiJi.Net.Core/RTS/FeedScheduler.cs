using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.RTS
{
    /// <summary>
    /// feed monitor scheduler
    /// </summary>
    public class FeedScheduler
    {
        private static IScheduler scheduler;
        private static StdSchedulerFactory factory;

        static FeedScheduler()
        {
            factory = new StdSchedulerFactory();
        }

        /// <summary>
        /// start scheduler
        /// </summary>
        public static async void Start(string cornExpression = "0 0/5 * * * ?", Dictionary<string,object> dic = null)
        {
            scheduler = await factory.GetScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<FeedJob>().Build();

            if(dic!=null)
            {
                foreach (var key in dic.Keys)
                {
                    job.JobDataMap.Add(key, dic[key]);
                }
            }

            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(cornExpression).Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// stop scheduler
        /// </summary>
        public static async void Stop()
        {
            await scheduler.Shutdown(false);
        }
    }
}
