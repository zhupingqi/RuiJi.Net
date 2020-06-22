using Newtonsoft.Json;
using Quartz;
using RuiJi.Net.Core.Code.Compiler;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Queue;
using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Node.Feed.Db;
using RuiJi.Net.Node.LTS;
using System;
using System.IO;
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

        internal static TaskQueuePool queuePool;

        private string baseUrl;
        //private string proxyUrl;

        static FeedJob()
        {
            basePath = AppDomain.CurrentDomain.BaseDirectory;
            snapshotPath = Path.Combine(basePath, "snapshot");
            delayPath = Path.Combine(basePath + "delay");

            queuePool = new TaskQueuePool(8);
            queuePool.Start();
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
            if (response == null)
            {
                Logger.GetLogger(baseUrl).Error(feedRequest.Request.Uri + " response save response is null.");
                return;
            }
            var request = feedRequest.Request;
            var content = Convert(response.Data.ToString(), Encoding.GetEncoding(response.Charset), Encoding.UTF8);

            var snap = new FeedSnapshot
            {
                Url = request.Uri.ToString(),
                Content = content,
                RuiJiExpression = feedRequest.Expression
            };

            var json = JsonConvert.SerializeObject(snap, Formatting.Indented);

            var fileName = Path.Combine(snapshotPath, feedRequest.Setting.Id + "_" + DateTime.Now.Ticks + ".json");
            if (feedRequest.Setting.Delay > 0)
            {
                fileName = Path.Combine(delayPath, feedRequest.Setting.Id + "_" + DateTime.Now.AddMinutes(feedRequest.Setting.Delay).Ticks + ".json");
            }

            Logger.GetLogger(baseUrl).Info(request.Uri + " response save to " + fileName);
            File.WriteAllText(fileName, json, Encoding.UTF8);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            baseUrl = context.JobDetail.JobDataMap.Get("baseUrl").ToString();
            var feedRequest = context.JobDetail.JobDataMap.Get("request") as FeedRequest;

            Logger.GetLogger(baseUrl).Info(" feed job " + context.JobDetail.Key + " add to feed crawl queue");

            var addrs = CodeCompilerManager.GetResult("url", feedRequest.Request.Uri.ToString());

            foreach (var addr in addrs)
            {
                queuePool.QueueAction(() =>
                {
                    Logger.GetLogger(baseUrl).Info(" feed job " + addr.ToString() + " starting");

                    feedRequest.Request = feedRequest.Request.Clone() as Request;
                    feedRequest.Request.Uri = new Uri(addr.ToString());

                    var response = DoTask(feedRequest);
                    Save(feedRequest, response);
                });
            }
        }
    }
}