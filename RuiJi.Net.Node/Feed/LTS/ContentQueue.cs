using Amib.Threading;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Queue;
using RuiJi.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Net.NodeVisitor;

namespace RuiJi.Net.Node.Feed.LTS
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
                    try
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
                    }
                    catch { }
                });
            }
        }

        public List<ExtractResult> Extract(string url)
        {
            var cralwer = new RuiJi.Net.NodeVisitor.Crawler();
            var response = cralwer.Request(url);
            var content = response.Data.ToString();

            var results = new List<ExtractResult>();

            var blocks = Feeder.GetExtractBlock(url);
            blocks.ForEach((m)=> {
                var r = RuiJi.Net.NodeVisitor.Extracter.Extract(new ExtractRequest
                {
                    Block = m,
                    Content = content
                });

                results.Add(r);
            });

            return results;
        }

        private void Save(List<ExtractResult> articles)
        {
            throw new NotImplementedException();
        }

        internal void Enqueue(string v)
        {
            queue.Enqueue(v);
        }
    }
}