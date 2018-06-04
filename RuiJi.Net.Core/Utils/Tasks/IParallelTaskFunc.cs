using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Tasks
{
    public interface IParallelTaskFunc
    {
        object Run(object t, ParallelTask task);
    }
}