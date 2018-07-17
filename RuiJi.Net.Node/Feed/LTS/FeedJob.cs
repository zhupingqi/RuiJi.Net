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

        private static string basePath;
        private static readonly string delayPath;
        private static readonly string snapshotPath;

        internal static SmartThreadPool smartThreadPool;

        private string baseUrl;
        private string proxyUrl;

        static FeedJob()
        {
            basePath = AppDomain.CurrentDomain.BaseDirectory;
            snapshotPath = Path.Combine(basePath, "snapshot");
            delayPath = Path.Combine(basePath + "delay");

            var stpStartInfo = new STPStartInfo
            {
                IdleTimeout = 3000,
                MaxWorkerThreads = 32,
                MinWorkerThreads = 0
            };

            smartThreadPool = new SmartThreadPool(stpStartInfo);
            smartThreadPool.Start();
        }

        private string Convert(string input, Encoding source, Encoding target)
        {
            var bytes = source.GetBytes(input);
            var dst = Encoding.Convert(source, target, bytes);
            return target.GetString(dst);
        }

        public Response DoTask(FeedModel feed)
        {
            return DoTask(FeedModel.ToFeedRequest(feed));
        }

        public Response DoTask(FeedRequest feedRequest)
        {
            try
            {
                var request = feedRequest.Request;

                Logger.GetLogger(baseUrl).Info("do task -> request address " + request.Uri);

                var response = NodeVisitor.Crawler.Request(request);

                if (response != null)
                    Logger.GetLogger(baseUrl).Info("request " + request.Uri + " response code is " + response.StatusCode);

                if (response == null)
                    Logger.GetLogger(baseUrl).Error("request " + request.Uri + " response is null");

                return response;
            }
            catch (Exception ex)
            {
                Logger.GetLogger(baseUrl).Info("do task -> request address failed " + ex.Message);
            }

            return null;
        }

        protected void Save(FeedRequest feedRequest, Response response)
        {
            var request = feedRequest.Request;
            var content = Convert(response.Data.ToString(), Encoding.GetEncoding(response.Charset), Encoding.UTF8);

            var snap = new FeedSnapshot
            {
                Url = request.Uri.ToString(),
                Content = content,
                RuiJiExpression = feedRequest.Expression
            };

            var json = JsonConvert.SerializeObject(snap, Formatting.Indented);

            var fileName = Path.Combine(basePath, feedRequest.Setting.Id + "_" + DateTime.Now.Ticks + ".json");
            if (feedRequest.Setting.Delay > 0)
            {
                fileName = Path.Combine(basePath, feedRequest.Setting.Id + "_" + DateTime.Now.AddMinutes(feedRequest.Setting.Delay).Ticks + ".json");
            }

            Logger.GetLogger(baseUrl).Info(request.Uri + " response save to " + fileName);
            File.WriteAllText(fileName, json, Encoding.UTF8);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            baseUrl = context.JobDetail.JobDataMap.Get("baseUrl").ToString();
            proxyUrl = context.JobDetail.JobDataMap.Get("proxyUrl").ToString();
            var feedRequest = context.JobDetail.JobDataMap.Get("request") as FeedRequest;

            Logger.GetLogger(baseUrl).Info(" feed job " + context.JobDetail.Key + " add to feed crawl queue");

            smartThreadPool.QueueWorkItem(() =>
            {
                Logger.GetLogger(baseUrl).Info(" feed job " + feedRequest.Request.Uri.ToString() + " starting");

                var response = DoTask(feedRequest);
                Save(feedRequest, response);
            });
        }
    }
}