using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Core.Utils.Tasks
{
    public class ParallelTaskManager
    {
        private static object _lck = new object();

        private static Dictionary<int, ParallelTask> tasks;
        private static TaskFactory facotry;

        static ParallelTaskManager()
        {
            tasks = new Dictionary<int, ParallelTask>();
            facotry = new TaskFactory();
        }

        private ParallelTaskManager()
        {
            
        }

        public static ParallelTask StartNew<T>(object arg) where T : IParallelTaskFunc, new()
        {
            if (arg == null)
                return null;

            RemoveTimeout();

            lock (_lck)
            {
                var pTask = new ParallelTask();
                pTask.Task = facotry.StartNew<object>(() => {
                    pTask.Func = new T();

                    return RunTask<T>(arg, pTask);
                });

                tasks.Add(pTask.Task.Id, pTask);

                return pTask;
            }
        }

        public static ParallelTask Get(int taskId)
        {
            lock (_lck)
            {
                if (tasks.ContainsKey(taskId))
                    return tasks[taskId];

                return null;
            }
        }

        public static bool Remove(int taskId)
        {
            lock (_lck)
            {
                tasks[taskId].CancelToken.Cancel();
                tasks[taskId].Task = null;

                return tasks.Remove(taskId);
            }
        }

        public static int[] RemoveTimeout()
        {
            lock (_lck)
            {
                var ids = tasks.Where(m => m.Value.StartTime.AddMinutes(15) < DateTime.Now).Select(m => m.Key).ToArray();
                foreach (var id in ids)
                {
                    tasks.Remove(id);
                }

                return ids;
            }
        }

        private static object RunTask<T>(object arg, ParallelTask task)
        {
            Thread thread = Thread.CurrentThread;
            task.CancelToken.Token.Register(() =>
            {
                thread.Abort();
            });

            return task.Func.Run(arg, task);
        }
    }
}