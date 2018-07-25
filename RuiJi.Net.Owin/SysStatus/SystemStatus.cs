using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RuiJi.Net.Owin.SysStatus
{
    public abstract class SystemStatus
    {
        public SystemStatus()
        {
            OS = RuntimeInformation.OSDescription + " " + RuntimeInformation.OSArchitecture;
            CpuCores = System.Environment.ProcessorCount;
            Environment = Microsoft.Extensions.DependencyModel.DependencyContext.Default.Target.Framework;
        }
        public string OS { get; set; }

        public string Environment { get; set; }

        public string Cpu { get; set; }

        public int CpuCores { get; set; }

        public float Memory { get; set; }

        public abstract float CpuUsage();

        public abstract float MemoryUsage();

        public abstract object NetworkUsage();
    }
}
