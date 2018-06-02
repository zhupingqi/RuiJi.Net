using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Tasks
{
    public interface ITask<TResult, TProgress>
    {
        DateTime StartTime { get; set; }
        CancellationTokenSource CancelToken { get; set; }
        IProgress<TProgress> Progress { get; set; }
        Task<TResult> Task { get; set; }
    }
}