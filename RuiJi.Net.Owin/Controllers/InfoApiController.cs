using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

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
            
            return new { memoryLoad = memoryLoad, cpuLoad = cpuLoad };
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

            return new { nodeType = server.NodeType.ToString(), startTime = server.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), cpu = sys.ProcessorName, memory = memory, efVersion = Environment.Version.ToString() };
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
