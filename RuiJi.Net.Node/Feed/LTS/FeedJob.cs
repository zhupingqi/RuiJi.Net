using Amib.Threading;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.RTS;
using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Node.Compile;
using RuiJi.Net.Node.Feed.Db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class FeedJob : IJob
    {
        public static bool IsRunning = false;

        private static string baseDir;

        private string baseUrl;
        private string proxyUrl;

        static FeedJob()
        {
            baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(baseDir + @"snapshot"))
            {
                Directory.CreateDirectory(baseDir + @"snapshot");
            }

            if (!Directory.Exists(baseDir + @"delay"))
            {
                Directory.CreateDirectory(baseDir + @"delay");
            }
        }

        private string Convert(string input, Encoding source, Encoding target)
        {
            var bytes = source.GetBytes(input);
            var dst = Encoding.Convert(source, target, bytes);
            return target.GetString(dst);
        }

        public Response DoTask(FeedModel feed)
        {
            //如何Execute没执行过Test时baseUrl会不会为Null？
            return FeedQueue.Instance.DoTask(FeedModel.ToFeedRequest(feed), baseUrl);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            baseUrl = context.JobDetail.JobDataMap.Get("baseUrl").ToString();
            proxyUrl = context.JobDetail.JobDataMap.Get("proxyUrl").ToString();

            await Task.Run(() =>
            {
                var feedRequest = context.JobDetail.JobDataMap.Get("request") as FeedRequest;
                FeedQueue.Instance.Enqueue(new FeedQueueModel { BaseDir = baseDir, BaseUrl = baseUrl, FeedRequest = feedRequest });
            });
        }
    }
}