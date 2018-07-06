using Amib.Threading;
using Newtonsoft.Json;
using Quartz;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Selector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.RTS
{
    public abstract class FeedExtractJobBase<T> : IJob
    {
        private static bool IsRunning = false;

        public static int MaxWorkerThreads { get; set; }

        static FeedExtractJobBase()
        {
            MaxWorkerThreads = 8;            
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (!IsRunning)
            {
                IsRunning = true;

                OnStart(context);

                var task = Task.Factory.StartNew(() =>
                {
                    var snapshots = GetSnapshot();

                    var stpStartInfo = new STPStartInfo
                    {
                        IdleTimeout = 3000,
                        MaxWorkerThreads = MaxWorkerThreads,
                        MinWorkerThreads = 0
                    };

                    var pool = new SmartThreadPool(stpStartInfo);
                    var waits = new List<IWorkItemResult>();
                    foreach (var snapshot in snapshots)
                    {
                        var item = pool.QueueWorkItem((u) =>
                        {
                            DoTask(u);

                        }, snapshot);

                        waits.Add(item);
                    }

                    SmartThreadPool.WaitAll(waits.ToArray());
                    pool.Shutdown(true, 1000);
                    pool.Dispose();
                    pool = null;
                    waits.Clear();
                });

                await task;

                IsRunning = false;
            }
        }

        protected virtual void OnStart(IJobExecutionContext context)
        {
            
        }

        protected abstract List<T> GetSnapshot();

        protected abstract void DoTask(T snapshot);

        protected abstract void Save(string url);
    }
}