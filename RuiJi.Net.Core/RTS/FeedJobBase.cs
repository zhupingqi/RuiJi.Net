using Amib.Threading;
using Newtonsoft.Json;
using Quartz;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Utils.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.RTS
{
    /// <summary>
    /// feed job base class
    /// </summary>
    public abstract class FeedJobBase : IJob
    {
        private static bool IsRunning = false;

        public static int MaxWorkerThreads { get; set; }

        static FeedJobBase()
        {
            MaxWorkerThreads = 8;
        }

        /// <summary>
        /// execute job
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            if (!IsRunning)
            {
                IsRunning = true;

                OnJobStart(context);

                var task = Task.Factory.StartNew(() =>
                {
                    var requests = GetRequests();

                    var stpStartInfo = new STPStartInfo
                    {
                        IdleTimeout = 3000,
                        MaxWorkerThreads = MaxWorkerThreads,
                        MinWorkerThreads = 0
                    };

                    var pool = new SmartThreadPool(stpStartInfo);
                    var waits = new List<IWorkItemResult>();

                    foreach (var fr in requests)
                    {
                        if (fr.Request.Headers.Count(m => m.Name == "Referer") == 0)
                            fr.Request.Headers.Add(new WebHeader("Referer", fr.Request.Uri.AbsoluteUri));

                        var item = pool.QueueWorkItem((u) =>
                        {
                            var response = DoTask(u);
                            Save(u, response);
                        }, fr);

                        waits.Add(item);
                    }

                    SmartThreadPool.WaitAll(waits.ToArray());

                    pool.Shutdown(true, 1000);
                    pool.Dispose();
                    pool = null;
                    waits.Clear();
                });

                await task;

                OnJobEnd();

                IsRunning = false;
            }
        }

        /// <summary>
        /// convert text encoding
        /// </summary>
        /// <param name="input">text need to be convert</param>
        /// <param name="source">source encoding</param>
        /// <param name="target">target encoding</param>
        /// <returns>text encoded</returns>
        protected string ConvertEncoding(string input, Encoding source, Encoding target)
        {
            var bytes = source.GetBytes(input);
            var dst = Encoding.Convert(source, target, bytes);
            return target.GetString(dst);
        }

        /// <summary>
        /// execute on job start
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnJobStart(IJobExecutionContext context)
        {

        }

        /// <summary>
        /// execute on job end
        /// </summary>
        protected virtual void OnJobEnd()
        { }

        /// <summary>
        /// get feed request
        /// </summary>
        /// <returns>feed request list</returns>
        protected abstract List<FeedRequest> GetRequests();

        /// <summary>
        /// execute crawl
        /// </summary>
        /// <param name="feedRequest"></param>
        /// <returns>crawl response</returns>
        public abstract Response DoTask(FeedRequest feedRequest);

        /// <summary>
        /// save feed snapshot
        /// </summary>
        /// <param name="feedRequest">feedRequest</param>
        /// <param name="response">crawl response</param>
        protected abstract void Save(FeedRequest feedRequest, Response response);
    }
}