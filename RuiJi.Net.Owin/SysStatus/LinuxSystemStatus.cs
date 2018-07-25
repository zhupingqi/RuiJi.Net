using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace RuiJi.Net.Owin.SysStatus
{
    public class LinuxSystemStatus : SystemStatus
    {
        public override float CpuUsage()
        {
            var infos = File.ReadAllLines("/proc/stat");

            var cpu1 = infos[0].Split(' ');

            int user1 = int.Parse(cpu1[2]);
            int nice1 = int.Parse(cpu1[3]);
            int sys1 = int.Parse(cpu1[4]);
            int idle1 = int.Parse(cpu1[5]);
            var total1 = user1 + nice1 + sys1;

            Thread.Sleep(1000);

            infos = File.ReadAllLines("/proc/stat");
            var cpu2 = infos[0].Split(' ');
            int user2 = int.Parse(cpu2[2]);
            int nice2 = int.Parse(cpu2[3]);
            int sys2 = int.Parse(cpu2[4]);
            int idle2 = int.Parse(cpu2[5]);
            var total2 = user2 + nice2 + sys2;

            return 1-(total2 - total1) / (total2 - total1 + idle2 - idle1) * 100;
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
