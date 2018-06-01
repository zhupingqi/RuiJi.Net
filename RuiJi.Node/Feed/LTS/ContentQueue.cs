using Amib.Threading;
using RuiJi.Core.Extracter;
using RuiJi.Core.Queue;
using RuiJi.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Feed.LTS
{
    public class ContentQueue
    {
        private static ContentQueue contentQueue;

        private MessageQueue<string> queue;
        private SmartThreadPool pool;
        private STPStartInfo stpStartInfo;

        static ContentQueue()
        {
            contentQueue = new ContentQueue();
        }

        private ContentQueue()
        {
            queue = new MessageQueue<string>();
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

        private void queue_ContentChanged(object sender, QueueChangedEventArgs<string> args)
        {
            if (args.Action == QueueChangedActionEnum.Enqueue)
            {
                pool.QueueWorkItem(() =>
                {
                    string url;
                    if (queue.TryDequeue(out url))
                    {
                        var article = Extract(url);
                        if (article != null)
                        {
                            Save(article);
                        }
                    }
                });
            }
        }

        public List<ExtractResult> Extract(string url)
        {
            var cralwer = new RuiJi.Net.Crawler();
            var response = cralwer.Request(url);
            var content = response.Data.ToString();

            var results = new List<ExtractResult>();

            var blocks = Feeder.GetExtractBlock(url);
            blocks.ForEach((m)=> {
                var r = RuiJi.Net.Extracter.Extract(new ExtractRequest
                {
                    Block = m,
                    Content = content
                });

                results.Add(r);
            });

            return results;
        }

        private void Save(object article)
        {
            throw new NotImplementedException();
        }

        internal void Enqueue(string v)
        {
            queue.Enqueue(v);
        }
    }
}