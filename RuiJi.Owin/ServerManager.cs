using Microsoft.Owin.Hosting;
using RuiJi.Node;
using RuiJi.Node.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Owin
{
    public class ServerManager
    {
        private static List<Task> tasks;
        private static List<WebApiServer> servers;
        private static Process zkProcess;

        static ServerManager()
        {
            servers = new List<WebApiServer>();
            tasks = new List<Task>();
        }

        ~ServerManager()
        {
            StopAll();
            if(zkProcess!= null)
                zkProcess.Kill();
        }

        public static void Start(string baseUrl, string type, string zkServer, string proxy = "")
        {
            var server = new WebApiServer();
            servers.Add(server);

            server.Start(baseUrl, type, zkServer, proxy);
        }

        public static void Stop(string port = "")
        {
            var server = servers.SingleOrDefault(m => m.Port == port);
            if (server != null)
            {
                server.Stop();
                //servers.Remove(server);

                Console.WriteLine("server port with " + port + " stop!");
            }

            tasks.RemoveAll(m=>m.Status != TaskStatus.Running);
        }

        internal static WebApiServer GetNode(string port)
        {
            return servers.SingleOrDefault(m => m.Port == port);
        }

        public static void StartServers()
        {
            var zkServer = ConfigurationManager.AppSettings["zkServer"];
            if(!string.IsNullOrEmpty(zkServer))
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + zkServer + @"\bin\zkServer.cmd";

                zkProcess = new Process();
                zkProcess.StartInfo.FileName = path;
                zkProcess.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                zkProcess.StartInfo.RedirectStandardInput = false;//接受来自调用程序的输入信息
                zkProcess.StartInfo.RedirectStandardOutput = false;//由调用程序获取输出信息
                zkProcess.StartInfo.RedirectStandardError = false;//重定向标准错误输出
                zkProcess.StartInfo.CreateNoWindow = false;//不显示程序窗口
                zkProcess.Start();//启动程序
            }

            Thread.Sleep(3000);

            NodeConfigurationSection.Settings.ForEach(m =>
            {
                var t = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ServerManager.Start(m.BaseUrl, m.Type, m.ZkServer, m.Proxy);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                tasks.Add(t);
                Thread.Sleep(1000);
            });
        }

        public static void Start(string port)
        {
            var server = servers.SingleOrDefault(m => m.Port == port);
            if (server != null)
            {
                if (server.Running)
                {
                    Console.WriteLine("server " + server.NodeBase.BaseUrl + " already running!");
                }
                else
                {
                    var t = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            server.Restart();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    });

                    tasks.Add(t);
                    
                    Console.WriteLine("server " + server.NodeBase.BaseUrl + " restart!");
                }
            }
            else
            {
                Console.WriteLine("server not find");
            }
        }

        public static void StopAll()
        {
            if (zkProcess != null)
            {
                zkProcess.Kill();
                zkProcess = null;
            }

            servers.ForEach(m =>
            {
                m.Stop();
                Console.WriteLine("server port with " + m.Port + " stop!");
            });
            servers.Clear();

            Console.WriteLine("all server stop!");
        }

        public static NodeBase GetLeader()
        {
            var server = servers.SingleOrDefault(m => m.NodeBase.IsLeader);
            if (server != null)
                return server.NodeBase;

            return null;
        }
    }
}