using Amib.Threading;
using Newtonsoft.Json;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Queue;
using RuiJi.Net.Core.RTS;
using RuiJi.Net.Core.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class FeedQueueModel
    {

        public FeedRequest FeedRequest { get; set; }

        public string BaseUrl { get; set; }

        public string BaseDir { get; set; }
    }
    public class FeedQueue
    {
        private static FeedQueue feedQueue;

        private MessageQueue<FeedQueueModel> queue;
        private SmartThreadPool pool;
        private STPStartInfo stpStartInfo;
        private string path;

        static FeedQueue()
        {
            feedQueue = new FeedQueue();
        }

        private FeedQueue()
        {
            path = AppDomain.CurrentDomain.BaseDirectory + "save_failed";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);



            queue = new MessageQueue<FeedQueueModel>();
            queue.ContentChanged += queue_ContentChanged;

            stpStartInfo = new STPStartInfo
            {
                IdleTimeout = 3000,
                MaxWorkerThreads = 8,
                MinWorkerThreads = 0
            };

            pool = new SmartThreadPool(stpStartInfo);
        }

        public static FeedQueue Instance
        {
            get
            {
                return feedQueue;
            }
        }

        private void queue_ContentChanged(object sender, QueueChangedEventArgs<FeedQueueModel> args)
        {
            if (args.Action == QueueChangedActionEnum.Enqueue)
            {
                pool.QueueWorkItem(() =>
                {
                    FeedQueueModel qm;
                    if (queue.Dequeue(out qm))
                    {
                        //crawler the feed
                        var response = DoTask(qm.FeedRequest, qm.BaseUrl);

                        if (response != null)
                        {
                            Save(qm.FeedRequest, response, qm.BaseDir, qm.BaseUrl);
                        }
                    }

                });
            }
        }

        internal void Enqueue(FeedQueueModel v)
        {
            if (queue.Count(q => q.FeedRequest.Setting.Id == v.FeedRequest.Setting.Id) == 0)
            {
                queue.Enqueue(v);
            }
        }

        public Response DoTask(FeedRequest feedRequest, string baseUrl)
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

        protected void Save(FeedRequest feedRequest, Response response, string baseDir, string baseUrl)
        {
            var fileName = baseDir + @"snapshot\" + feedRequest.Setting.Id + "_" + DateTime.Now.Ticks + ".json";
            var request = feedRequest.Request;
            try
            {
                var content = Convert(response.Data.ToString(), Encoding.GetEncoding(response.Charset), Encoding.UTF8);

                var snap = new FeedSnapshot
                {
                    Url = request.Uri.ToString(),
                    Content = content,
                    RuiJiExpression = feedRequest.Expression
                };

                var json = JsonConvert.SerializeObject(snap, Formatting.Indented);


                if (feedRequest.Setting.Delay > 0)
                {
                    fileName = baseDir + @"delay\" + feedRequest.Setting.Id + "_" + DateTime.Now.AddMinutes(feedRequest.Setting.Delay).Ticks + ".json";
                }

                Logger.GetLogger(baseUrl).Info(request.Uri + " response save to " + fileName);
                File.WriteAllText(fileName, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Logger.GetLogger(baseUrl).Info("save feed ->" + request.Uri + " response save to " + fileName + " failed " + ex.Message);
            }

        }

        private string Convert(string input, Encoding source, Encoding target)
        {
            var bytes = source.GetBytes(input);
            var dst = Encoding.Convert(source, target, bytes);
            return target.GetString(dst);
        }
    }
}
