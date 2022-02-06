using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Queue
{
    public sealed class TaskQueuePool : ConcurrentQueue<QueueActionDelegate>
    {
        private AutoResetEvent resetEvent = new AutoResetEvent(false);

        private bool shutdown = false;
        private Task mainTask;
        private long currentTasks = 0;

        public int MaxTasks { get; private set; }

        public long CurrentTasks
        {
            get
            {
                return currentTasks;
            }
        }

        public TaskQueuePool(int maxWorkerThreads = 32)
        {
            this.MaxTasks = maxWorkerThreads;
        }

        public void Start()
        {
            mainTask = Task.Run(() =>
            {
                while (!shutdown)
                {
                    var result = Dequeue();

                    if (result == null)
                        continue;

                    Task.Run(() =>
                    {
                        try
                        {
                            if (result.Args != null)
                                result.Action.DynamicInvoke(result.Args);
                            else
                                result.Action.DynamicInvoke();
                        }
                        catch
                        {
                        }

                        OnTaskComplete();
                    });
                }
            });
        }

        public void Shutdown()
        {
            shutdown = true;
            if (mainTask != null)
                mainTask.Dispose();
        }

        private new void Enqueue(QueueActionDelegate @delegate)
        {
            lock (this)
            {
                base.Enqueue(new QueueActionDelegate { Action = @delegate.Action, Args = @delegate.Args });

                if (currentTasks < MaxTasks)
                    resetEvent.Set();
                else
                    resetEvent.Reset();
            }
        }

        private QueueActionDelegate Dequeue()
        {
            resetEvent.WaitOne();

            QueueActionDelegate action;

            if (base.TryDequeue(out action))
            {
                Interlocked.Increment(ref currentTasks);
                //Debug.WriteLine("Dequeue -> " + action.Args.ToString());

                return action;
            }

            return null;
        }

        private void OnTaskComplete()
        {
            lock (this)
            {
                Interlocked.Decrement(ref currentTasks);
                resetEvent.Set();
            }
        }

        public void QueueAction(Action action)
        {
            lock (this)
            {
                Enqueue(new QueueActionDelegate { Action = action });
            }
        }

        public void QueueAction<T>(Action<T> action, T args)
        {
            lock (this)
            {
                Enqueue(new QueueActionDelegate { Action = action, Args = args });
            }
        }

        public void WaitAll()
        {
            Task.Run(() => {
                while (!base.IsEmpty || currentTasks > 0)
                {
                    Thread.Sleep(100);
                }
            }).Wait();
        }
    }
}