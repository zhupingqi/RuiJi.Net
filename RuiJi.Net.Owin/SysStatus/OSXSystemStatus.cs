using System;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Owin.SysStatus
{
    public class OSXSystemStatus : SystemStatus
    {
        public override float CpuUsage()
        {
            throw new NotImplementedException();
        }

        public override float MemoryUsage()
        {
            throw new NotImplementedException();
        }

        public override object NetworkUsage()
        {
            throw new NotImplementedException();
        }
    }
}
