using Amib.Threading;
using Newtonsoft.Json;
using RuiJi.Net.Core.Queue;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Storage;
using RuiJi.Net.Storage.Model;
using System;
using System.IO;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class QueueModel
    {
        public string Url { get; set; }

        public int FeedId { get; set; }
    }

    public class ContentQueue
    {
        private static ContentQueue contentQueue;

        private MessageQueue<QueueModel> queue;
        private SmartThreadPool pool;
        private STPStartInfo stpStartInfo;
        private string path;

        static ContentQueue()
        {
            contentQueue = new ContentQueue();
        }

        private ContentQueue()
        {
            path = AppDomain.CurrentDomain.BaseDirectory + "save_failed";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            

            queue = new MessageQueue<QueueModel>();
            queue.ContentChanged += queue_ContentChanged;

            stpStartInfo = new STPStartInfo
            {
                IdleTimeout = 3000,
                MaxWorkerThreads = 8,
                MinWorkerThreads = 0
            };

            pool = new SmartThreadPool(stpStartInfo);
        }

        public static ContentQueue Instance
        {
            get
            {
                return contentQueue;
            }
        }

        private void queue_ContentChanged(object sender, QueueChangedEventArgs<QueueModel> args)
        {
            if (args.Action == QueueChangedActionEnum.Enqueue)
            {
                pool.QueueWorkItem(() =>
                {
                    try
                    {
                        QueueModel qm;
                        if (queue.Dequeue(out qm))
                        {
                            var result = NodeVisitor.Cooperater.GetResult(qm.Url);
                            if (result != null)
                            {
                                var cm = new ContentModel();
                                cm.FeedId = qm.FeedId;
                                cm.Url = qm.Url;
                                cm.Metas = result.Metas;
                                cm.CDate = DateTime.Now;

                                var connectString = string.Format(@"LiteDb/Content/{0}.db", DateTime.Now.ToString("yyyyMM"));
                                var storage = new LiteDbStorage(connectString, "contents");
                                if (storage.Insert(cm) == -1)
                                    File.AppendAllText(path + @"\" + EncryptHelper.GetMD5Hash(qm.Url) + ".json", JsonConvert.SerializeObject(cm));
                            }
                        }
                    }
                    catch {
                        //save failed
                    }
                });
            }
        }

        internal void Enqueue(QueueModel v)
        {
            queue.Enqueue(v);
        }
    }
}