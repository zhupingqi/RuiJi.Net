using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.Owin.SysStatus
{
    public class LinuxSystemStatus : SystemStatus
    {
        public LinuxSystemStatus()
        {
            Cpu = GetCpu();
            Memory = GetMemTotal();
        }

        private float GetMemTotal()
        {
            var infos = File.ReadAllLines("/proc/meminfo");

            return float.Parse(Regex.Split(infos[0], "\\s+")[1]) / 1024f;
        }

        private string GetCpu()
        {
            var infos = File.ReadAllLines("/proc/cpuinfo");
            var model_Name = infos.FirstOrDefault(i => i.Contains("model name"));
            return model_Name.Substring(model_Name.IndexOf(":") + 1);
        }
        public override Task<double> CpuUsage()
        {
            return Task.Run(() =>
            {
                var infos = File.ReadAllLines("/proc/stat");

                var cpu1 = Regex.Split(infos[0].Trim(), "\\s+");

                float user1 = float.Parse(cpu1[1]);
                float nice1 = float.Parse(cpu1[2]);
                float sys1 = float.Parse(cpu1[3]);
                float idle1 = float.Parse(cpu1[4]);
                float iowait1 = float.Parse(cpu1[5]);
                float irq1 = float.Parse(cpu1[6]);
                float softirq1 = float.Parse(cpu1[7]);
                float total1 = user1 + nice1 + sys1 + idle1 + iowait1 + irq1 + softirq1;
                Thread.Sleep(1000);

                infos = File.ReadAllLines("/proc/stat");
                var cpu2 = Regex.Split(infos[0].Trim(), "\\s+");
                float user2 = float.Parse(cpu2[1]);
                float nice2 = float.Parse(cpu2[2]);
                float sys2 = float.Parse(cpu2[3]);
                float idle2 = float.Parse(cpu2[4]);
                float iowait2 = float.Parse(cpu2[5]);
                float irq2 = float.Parse(cpu2[6]);
                float softirq2 = float.Parse(cpu2[7]);
                float total2 = user2 + nice2 + sys2 + idle2 + iowait2 + irq2 + softirq2;

                return (total2 - total1 <= 0) ? 0 : Math.Round((1 - ((idle2 - idle1) / (total2 - total1))) * 100);
            });
        }

        public override Task<double> MemoryUsage()
        {
            return Task.Run(() =>
            {
                var infos = File.ReadAllLines("/proc/meminfo");

                var availableMem = int.Parse(Regex.Split(infos.FirstOrDefault(i => i.StartsWith("MemAvailable:")), "\\s+")[1]);
                return Math.Round((1 - (availableMem / 1024 / Memory)) * 100);
            });
        }

        public override Task<object> NetworkThroughput()
        {
            return Task.Run(() =>
            {
                var infos = File.ReadAllLines("/proc/net/dev");

                float sentSum1 = 0;
                float receivedSum1 = 0;
                foreach (var info in infos)
                {
                    var net = info.Trim();
                    if (net.StartsWith("Inter-|") || net.StartsWith("face") || net.StartsWith("lo"))
                        continue;
                    var netArr = Regex.Split(net, "\\s+");
                    sentSum1 += float.Parse(netArr[1]);
                    receivedSum1 += float.Parse(netArr[9]);
                }

                Thread.Sleep(1000);

                infos = File.ReadAllLines("/proc/net/dev");

                float sentSum2 = 0;
                float receivedSum2 = 0;
                foreach (var info in infos)
                {
                    var net = info.Trim();
                    if (net.StartsWith("Inter-|") || net.StartsWith("face") || net.StartsWith("lo"))
                        continue;
                    var netArr = Regex.Split(net, "\\s+");
                    sentSum2 += float.Parse(netArr[1]);
                    receivedSum2 += float.Parse(netArr[9]);
                }

                return (object)new
                {
                    sent = Math.Round((sentSum2 - sentSum1) / 1024 / 1024, 2),
                    received = Math.Round((receivedSum2 - receivedSum1) / 1024 / 1024, 2)
                };
            });
        }
    }
}
