using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Vanara.PInvoke;
using static Vanara.PInvoke.IpHlpApi;

namespace RuiJi.Net.Owin.Controllers
{
    public class InfoApiController : ApiController
    {
        /// <summary>
        /// 获取系统信息（此方法应该刷新，还未完成）
        /// </summary>
        /// <returns>cpu 内存信息</returns>
        [HttpGet]
        [WebApiCacheAttribute(Duration = 0)]
        public object System()
        {
            SystemInfo sys = new SystemInfo();

            var memoryLoad = 100 - ((double)sys.MemoryAvailable / (double)sys.PhysicalMemory) * 100;

            var cpuLoad = sys.CpuLoad;

            var iftable1 = IpHlpApi.GetIfTable();
            long inSpeed1 = iftable1.Sum(m => m.dwInOctets);
            long outSpeed1 = iftable1.Sum(m => m.dwOutOctets);

            Thread.Sleep(1000);

            var iftable2 = IpHlpApi.GetIfTable();
            var inSpeed2 = iftable2.Sum(m => m.dwInOctets);
            var outSpeed2 = iftable2.Sum(m => m.dwOutOctets);

            var inSpeed = inSpeed2 - inSpeed1;
            var outSpeed = outSpeed2 - outSpeed1;

            var ada = IpHlpApi.GetInterfaceInfo();
            ulong total = 0;

            foreach (var a in ada.Adapter)
            {
                MIB_IF_ROW2 row = new MIB_IF_ROW2(a.Index);
                IpHlpApi.GetIfEntry2(ref row);

                if (row.InOctets > 0)
                {
                    total += row.ReceiveLinkSpeed;
                }
            }

            total = total / 8;

            return new {
                memoryLoad = memoryLoad,
                cpuLoad = cpuLoad,
                inSpeed = (double)inSpeed * 100 / Convert.ToDouble(total),
                outSpeed = (double)outSpeed * 100 / Convert.ToDouble(total)
            };
        }

        /// <summary>
        /// 配置文件中的baseurl为localhost的，暂时读不到，js注释了
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public object Server(string baseUrl)
        {
            baseUrl = new Uri(baseUrl).Authority;
            var server = ServerManager.Get(baseUrl);

            SystemInfo sys = new SystemInfo();
            var memory = Math.Round((double)sys.PhysicalMemory / 1024 / 1024 / 1024, 1, MidpointRounding.AwayFromZero) + "GB";           

            return new {
                nodeType = server.NodeType.ToString(),
                startTime = server.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                cpu = sys.ProcessorName,
                memory = memory,
                efVersion = sys.Version
            };
        }

        /// <summary>
        /// dll可以动态加载，是否可以写在appconfig中？
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object Dll()
        {
            var dlls = new string[] { "RuiJi.Net.Core", "RuiJi.Net.Node", "RuiJi.Net.NodeVisitor", "RuiJi.Net.Owin" };
            var versions = new List<string>();
            foreach (var dll in dlls)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + dll + ".dll";
                Assembly assembly = Assembly.LoadFile(path);
                AssemblyName assemblyName = assembly.GetName();
                Version version = assemblyName.Version;
                versions.Add(dll + " " + version.ToString());
            }
            return new { versions };
        }
    }
}
