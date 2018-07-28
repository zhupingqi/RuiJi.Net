using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Vanara.PInvoke.IpHlpApi;

namespace RuiJi.Net.Owin.SysStatus
{
    public class WindowsSystemStatus : SystemStatus
    {
        private PerformanceCounter CpuCounter { get; set; }

        private PerformanceCounter RamCounter { get; set; }

        PerformanceCounter[] SentCounters { get; set; }

        PerformanceCounter[] ReceivedCounters { get; set; }

        public WindowsSystemStatus()
        {
            try
            {
                using (var mc = new ManagementClass("Win32_Processor"))
                {
                    var moc = mc.GetInstances();
                    Cpu = moc.OfType<ManagementObject>()
                     .Select(mo => mo["Name"].ToString())
                     .FirstOrDefault();
                }
            }
            catch
            {
                Cpu = "Unable to obtain";
            }


            CpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            CpuCounter.NextValue();

            RamCounter = new PerformanceCounter("Memory", "Available MBytes");
            RamCounter.NextValue();

            PerformanceCounterCategory pcCategory = new PerformanceCounterCategory("Network Interface");
            string[] iNames = pcCategory.GetInstanceNames();

            SentCounters = iNames.Select(i => new PerformanceCounter("Network Interface", "Bytes Sent/sec", i)).ToArray();
            SentCounters.Sum(s => s.NextValue());
            ReceivedCounters = iNames.Select(i => new PerformanceCounter("Network Interface", "Bytes Received/sec", i)).ToArray();
            ReceivedCounters.Sum(r => r.NextValue());

            var memStatus = new MEMORYSTATUSEX();
            var installedMemory = GlobalMemoryStatusEx(memStatus) ? memStatus.ullTotalPhys / 1024 / 1024 : 0f;
            Memory = installedMemory;
        }
        public override Task<double> CpuUsage()
        {
            return Task.Run(() =>
            {
                return Math.Round(CpuCounter.NextValue());
            });
        }

        public override Task<double> MemoryUsage()
        {
            return Task.Run(() =>
            {
                return Math.Round((1 - RamCounter.NextValue() / Memory) * 100);
            });
        }

        public override Task<object> NetworkThroughput()
        {
            return Task.Run(() =>
            {
                return (object)new
                {
                    sent = Math.Round(SentCounters.Sum(s => s.NextValue()) / 1024 / 1024, 2),
                    received = Math.Round(ReceivedCounters.Sum(r => r.NextValue()) / 1024 / 1024, 2)
                };
            });
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
    }
}
