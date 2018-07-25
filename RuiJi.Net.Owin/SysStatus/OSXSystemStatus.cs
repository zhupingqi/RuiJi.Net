using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Owin.SysStatus
{
    public class OSXSystemStatus : SystemStatus
    {
        public override Task<double> CpuUsage()
        {
            throw new NotImplementedException();
        }

        public override Task<double> MemoryUsage()
        {
            throw new NotImplementedException();
        }

        public override Task<object> NetworkThroughput()
        {
            throw new NotImplementedException();
        }
    }
}
