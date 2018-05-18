using Newtonsoft.Json;
using Quartz;
using RuiJi.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regards.Web.Seed
{
    public class ProxyPingJob : IJob
    {
        public static bool IsRunning = false;

        public async Task Execute(IJobExecutionContext context)
        {
            if (!IsRunning)
            {
                IsRunning = true;

                await ProxyManager.Instance.RefreshStatus();

                IsRunning = false;
            }
        }
    }
}