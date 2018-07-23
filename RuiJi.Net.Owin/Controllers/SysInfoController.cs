using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace RuiJi.Net.Owin.Controllers
{
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
            //var sys = new SystemInfo();

            //sys.ReckonSpeed();

            //return new
            //{
            //    memoryLoad = 100 - ((double)sys.MemoryAvailable / (double)sys.PhysicalMemory) * 100,
            //    cpuLoad = sys.CpuLoad,
            //    inSpeed = (double)sys.InSpeed * 100 / Convert.ToDouble(sys.SpeedTotal),
            //    outSpeed = (double)sys.OutSpeed * 100 / Convert.ToDouble(sys.SpeedTotal)
            //};
            
            return new object();
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
            //var baseUrl = Request.Host.Value;
            //var server = ServerManager.Get(baseUrl);

            //var sys = new SystemInfo();
            //var memory = Math.Round((double)sys.PhysicalMemory / 1024 / 1024 / 1024, 1, MidpointRounding.AwayFromZero) + "GB";

            //return new
            //{
            //    nodeType = server.NodeType.ToString(),
            //    startTime = server.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            //    cpu = sys.ProcessorName,
            //    memory = memory,
            //    efVersion = sys.Version,
            //    cores = Environment.ProcessorCount
            //};

            return new object();
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
            
            //foreach (var dll in dlls)
            //{
            //    string path = AppDomain.CurrentDomain.BaseDirectory + dll + ".dll";
            //    if (!File.Exists(path))
            //        path = AppDomain.CurrentDomain.BaseDirectory + dll + ".exe";
            //    if (!File.Exists(path))
            //        continue;

            //    Assembly assembly = Assembly.LoadFile(path);
            //    AssemblyName assemblyName = assembly.GetName();
            //    Version version = assemblyName.Version;
            //    versions.Add(dll + " " + version.ToString());
            //}
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

            var handle = client.ExecuteAsync(restRequest,(restResponse)=> {
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
