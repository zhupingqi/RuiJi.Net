using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RuiJi.Core.Utils.Tasks
{
    public class ParallelTask
    {
        public DateTime StartTime { get; set; }

        [JsonIgnore]
        public CancellationTokenSource CancelToken { get; set; }

        [JsonIgnore]
        public Progress<string> Progress { get; set; }

        public Task<object> Task { get; set; }

        public int TaskId
        {
            get
            {
                return Task.Id;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return Task.IsCompleted;
            }
        }

        public string ProgressState
        {
            get;
            private set;
        }
        public IParallelTaskFunc Func { get; internal set; }

        public ParallelTask()
        {
            StartTime = DateTime.Now;
            CancelToken = new CancellationTokenSource();
            Progress = new Progress<string>();
            Progress.ProgressChanged += Progress_ProgressChanged;
        }

        private void Progress_ProgressChanged(object sender, string e)
        {
            ProgressState = e;
        }

        public bool Wait(int millisecondsTimeout)
        {
            return Task.Wait(millisecondsTimeout);
        }
    }
}