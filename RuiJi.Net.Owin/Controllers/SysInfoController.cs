using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Owin.SysStatus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.Owin.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/sys")]
    public class SysInfoController : ControllerBase
    {
        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <returns>cpu 内存信息</returns>
        [HttpGet]
        [Route("load")]
        public object SystemLoad()
        {
            var cputask = SystemStatusManager.Instance.CpuUsage();
            var memtask = SystemStatusManager.Instance.MemoryUsage();
            var networktask = SystemStatusManager.Instance.NetworkThroughput();
            Task.WaitAll(new Task[] { cputask, memtask, networktask });

            return new
            {
                memoryLoad = memtask.Result,
                cpuLoad = cputask.Result,
                netSpeed = networktask.Result,
            };
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("info")]
        public object Server()
        {
            var baseUrl = Request.Host.Value;
            var server = ServerManager.Get(baseUrl);

            var memory = Math.Round((double)SystemStatusManager.Instance.Memory / 1024, 1, MidpointRounding.AwayFromZero) + "GB";

            return new
            {
                nodeType = server.NodeType.ToString(),
                startTime = server.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                system = SystemStatusManager.Instance.OS,
                cpu = SystemStatusManager.Instance.Cpu,
                cores = SystemStatusManager.Instance.CpuCores,
                memory = memory,
                efVersion = SystemStatusManager.Instance.Environment

            };
        }

        /// <summary>
        /// 加载dll信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("dll")]
        public object Dll()
        {
            var dlls = new string[] { "RuiJi.Net.Core", "RuiJi.Net.Node", "RuiJi.Net.NodeVisitor", "RuiJi.Net.Owin" };
            var versions = new List<string>();

            foreach (var dll in dlls)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + dll + ".dll";
                if (!System.IO.File.Exists(path))
                    path = AppDomain.CurrentDomain.BaseDirectory + dll + ".exe";
                if (!System.IO.File.Exists(path))
                    continue;

                Assembly assembly = Assembly.LoadFile(path);
                AssemblyName assemblyName = assembly.GetName();
                Version version = assemblyName.Version;
                versions.Add(dll + " " + version.ToString());
            }
            return new { versions };
        }

        [HttpGet]
        [Route("~/api/github")]
        public object Pulse()
        {
            var client = new RestClient("https://github.com");
            var restRequest = new RestRequest("/zhupingqi/RuiJi.Net/pulse_committer_data/monthly");
            restRequest.Method = Method.GET;
            restRequest.JsonSerializer = new NewtonJsonSerializer();
            restRequest.AddHeader("Referer", "https://github.com/zhupingqi/RuiJi.Net/pulse");

            object response = new object();
            var resetEvent = new ManualResetEvent(false);

            var handle = client.ExecuteAsync(restRequest, (restResponse) =>
            {
                response = JsonConvert.DeserializeObject<object>(restResponse.Content);
                resetEvent.Set();
            });

            resetEvent.WaitOne();

            return response;
        }

        [HttpGet]
        [Route("~/api/alone")]
        public bool IsAlone()
        {
            return RuiJiConfiguration.Standalone;
        }
    }
}
