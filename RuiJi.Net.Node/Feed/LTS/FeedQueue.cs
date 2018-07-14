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
                        Response response = null;

                        //crawler the feed
                        var request = qm.FeedRequest.Request;
                        try
                        {
                            Logger.GetLogger(qm.BaseUrl).Info("do task -> request address " + request.Uri);

                            response = NodeVisitor.Crawler.Request(request);

                            if (response != null)
                                Logger.GetLogger(qm.BaseUrl).Info("request " + request.Uri + " response code is " + response.StatusCode);

                            if (response == null)
                            {
                                Logger.GetLogger(qm.BaseUrl).Error("request " + request.Uri + " response is null");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.GetLogger(qm.BaseUrl).Info("crawler feed -> request address failed " + ex.Message);
                            return;
                        }

                        //save the feed
                        var fileName = qm.BaseDir + @"snapshot\" + qm.FeedRequest.Setting.Id + "_" + DateTime.Now.Ticks + ".json";
                        try
                        {

                            var content = Convert(response.Data.ToString(), Encoding.GetEncoding(response.Charset), Encoding.UTF8);

                            var snap = new FeedSnapshot
                            {
                                Url = request.Uri.ToString(),
                                Content = content,
                                RuiJiExpression = qm.FeedRequest.Expression
                            };

                            var json = JsonConvert.SerializeObject(snap, Formatting.Indented);

                            if (qm.FeedRequest.Setting.Delay > 0)
                            {
                                fileName = qm.BaseDir + @"delay\" + qm.FeedRequest.Setting.Id + "_" + DateTime.Now.AddMinutes(qm.FeedRequest.Setting.Delay).Ticks + ".json";
                            }

                            Logger.GetLogger(qm.BaseUrl).Info(request.Uri + " response save to " + fileName);
                            File.WriteAllText(fileName, json, Encoding.UTF8);
                        }
                        catch (Exception ex)
                        {
                            Logger.GetLogger(qm.BaseUrl).Info("save feed ->" + request.Uri + " response save to " + fileName + " failed " + ex.Message);
                            throw;
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

        private string Convert(string input, Encoding source, Encoding target)
        {
            var bytes = source.GetBytes(input);
            var dst = Encoding.Convert(source, target, bytes);
            return target.GetString(dst);
        }
    }
}
