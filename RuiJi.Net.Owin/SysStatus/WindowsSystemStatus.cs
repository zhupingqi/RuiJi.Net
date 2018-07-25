using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static Vanara.PInvoke.IpHlpApi;

namespace RuiJi.Net.Owin.SysStatus
{
    public class WindowsSystemStatus : SystemStatus
    {
        private PerformanceCounter CpuCounter { get; set; }

        private PerformanceCounter RamCounter { get; set; }

        public WindowsSystemStatus()
        {
            try
            {
                var mc = new ManagementClass("Win32_Processor");
                var moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (mo["Name"] != null)
                    {
                        Cpu = mo["Name"].ToString();
                    }
                }
                moc.Dispose();
                mc.Dispose();
            }
            catch (Exception ex)
            {
                Cpu = "读取出错：" + ex.Message;
            }


            CpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            CpuCounter.NextValue();

            RamCounter = new PerformanceCounter("Memory", "Available MBytes");
            RamCounter.NextValue();

            var memStatus = new MEMORYSTATUSEX();
            var installedMemory = GlobalMemoryStatusEx(memStatus) ? memStatus.ullTotalPhys / 1024 / 1024 : 0f;
            Memory = installedMemory;
        }
        public override float CpuUsage()
        {
            return CpuCounter.NextValue();
        }

        public override float MemoryUsage()
        {
            return (Memory - RamCounter.NextValue()) / Memory * 100;
        }

        public override object NetworkUsage()
        {
            var iftable1 = GetIfTable();
            long inSpeed1 = iftable1.Sum(m => m.dwInOctets);
            long outSpeed1 = iftable1.Sum(m => m.dwOutOctets);

            Thread.Sleep(1000);

            var iftable2 = GetIfTable();
            var inSpeed2 = iftable2.Sum(m => m.dwInOctets);
            var outSpeed2 = iftable2.Sum(m => m.dwOutOctets);

            var m_InSpeed = inSpeed2 - inSpeed1;
            var m_OutSpeed = outSpeed2 - outSpeed1;

            var ada = GetInterfaceInfo();
            ulong total = 0;

            foreach (var a in ada.Adapter)
            {
                MIB_IF_ROW2 row = new MIB_IF_ROW2(a.Index);
                GetIfEntry2(ref row);

                if (row.InOctets > 0)
                {
                    total += row.ReceiveLinkSpeed;
                }
            }

            double m_SpeedTotal = total / 8;

            return new
            {
                inSpeed = m_InSpeed * 100 / m_SpeedTotal,
                outSpeed = m_OutSpeed * 100 / m_SpeedTotal
            };
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
