using Microsoft.Owin.Hosting;
using RuiJi.Net.Core.Utils.Log;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.Owin
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

                Logger.GetLogger("").Info("server port with " + port + " stop!");
            }

            tasks.RemoveAll(m=>m.Status != TaskStatus.Running);
        }

        internal static WebApiServer GetNode(string port)
        {
            return servers.SingleOrDefault(m => m.Port == port);
        }

        public static void StartServers()
        {
            var zkServer = ConfigurationManager.AppSettings["zkPath"];
            if (!string.IsNullOrEmpty(zkServer))
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + zkServer + @"\bin\zkServer.cmd";

                if (File.Exists(path))
                {
                    Logger.GetLogger("").Info("start up embed zookeeper");

                    zkProcess = new Process();
                    zkProcess.StartInfo.FileName = path;
                    zkProcess.StartInfo.UseShellExecute = false;
                    zkProcess.StartInfo.RedirectStandardInput = false;
                    zkProcess.StartInfo.RedirectStandardOutput = false;
                    zkProcess.StartInfo.RedirectStandardError = false;
                    zkProcess.StartInfo.CreateNoWindow = false;
                    zkProcess.Start();
                }
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
                        Logger.GetLogger("").Info(ex.Message);
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
                    Logger.GetLogger("").Info("server " + server.NodeBase.BaseUrl + " already running!");
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
                            Logger.GetLogger("").Info(ex.Message);
                        }
                    });

                    tasks.Add(t);

                    Logger.GetLogger("").Info("server " + server.NodeBase.BaseUrl + " restart!");
                }
            }
            else
            {
                Logger.GetLogger("").Info("server not find");
            }
        }

        public static void StopAll()
        {
            try
            {
                if (zkProcess != null)
                {
                    zkProcess.Kill();
                    zkProcess = null;
                }

                servers.ForEach(m =>
                {
                    m.Stop();
                    Logger.GetLogger("").Info("server port with " + m.Port + " stop!");
                });
                servers.Clear();
            }
            catch(Exception ex) {
                Logger.GetLogger("").Info(ex.Message);
            }

            Logger.GetLogger("").Info("all server stop!");
        }

        public static NodeBase GetLeader()
        {
            var server = servers.SingleOrDefault(m => m.NodeBase.IsLeader);
            if (server != null)
                return server.NodeBase;

            return null;
        }

        public static NodeBase Get(string baseUrl)
        {
            var temp = servers.SingleOrDefault(m => m.NodeBase.BaseUrl.ToLower() == baseUrl.ToLower());
            return servers.SingleOrDefault(m=>m.NodeBase.BaseUrl.ToLower() == baseUrl.ToLower()).NodeBase;            
        }
    }
}