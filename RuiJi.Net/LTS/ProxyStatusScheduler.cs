using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regards.Web.Seed
{
    public class ProxyStatusScheduler
    {
        private static IScheduler scheduler;
        private static StdSchedulerFactory factory;

        static ProxyStatusScheduler()
        {
            factory = new StdSchedulerFactory();
        }

        public static async void Start()
        {
            scheduler = await factory.GetScheduler();
            await scheduler.Start();


            IJobDetail job = JobBuilder.Create<ProxyPingJob>().Build();

            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule("0 0/1 * * * ?").Build();

            await scheduler.ScheduleJob(job,trigger);
        }

        public static async void Stop()
        {
            await scheduler.Shutdown(false);
        }
    }
}