using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Core.Utils.Tasks
{
    public abstract class ParallelTaskBase
    {
        public DateTime StartTime { get; set; }

        public CancellationTokenSource CancelToken { get; set; }

        protected ParallelTaskBase()
        {
            StartTime = DateTime.Now;
            CancelToken = new CancellationTokenSource();
        }
    }
}