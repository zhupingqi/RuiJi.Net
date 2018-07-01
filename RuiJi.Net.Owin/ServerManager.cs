using Microsoft.Owin.Hosting;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Utils.Log;
using RuiJi.Net.Node;
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

        public static void StartServers()
        {
            if (NodeConfigurationSection.Alone)
            {
                var baseUrl = ConfigurationManager.AppSettings["RuiJiServer"];
                if(string.IsNullOrEmpty(baseUrl))
                {
                    Logger.GetLogger("").Info("RuiJiServer not exsit in AppSettings");
                    return;
                }

                var t = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ServerManager.Start(baseUrl);
                    }
                    catch (Exception ex)
                    {
                        Logger.GetLogger("").Fatal(ex.Message);
                    }
                });

                tasks.Add(t);
            }
            else
            {
                var zkServer = ConfigurationManager.AppSettings["zkServer"];
                if(string.IsNullOrEmpty(zkServer))
                {
                    Logger.GetLogger("").Fatal("zkServer not defined");
                    return;
                }

                var zkPath = ConfigurationManager.AppSettings["zkPath"];
                if (!string.IsNullOrEmpty(zkPath))
                {
                    var path = AppDomain.CurrentDomain.BaseDirectory + zkPath + @"\bin\zkServer.cmd";

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
                            ServerManager.Start(m.BaseUrl, m.Type, zkServer, m.Proxy);
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
        }

        public static void Start(int port)
        {
            var server = servers.SingleOrDefault(m => m.Port == port.ToString());
            if (server != null)
            {
                if (server.Running)
                {
                    Logger.GetLogger("").Info("server " + server.Node.BaseUrl + " already running!");
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

                    Logger.GetLogger("").Info("server " + server.Node.BaseUrl + " restart!");
                }
            }
            else
            {
                Logger.GetLogger("").Info("server not find");
            }
        }

        public static void Start(string baseUrl)
        {
            var server = new WebApiServer();
            servers.Add(server);

            server.Start(baseUrl);
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
            var server = servers.SingleOrDefault(m => m.Node.IsLeader);
            if (server != null)
                return (NodeBase)server.Node;

            return null;
        }

        public static INode Get(string baseUrl)
        {
            //aliyun nat...
            baseUrl = baseUrl.Replace("118.31.61.230", "172.16.50.52");

            var temp = servers.SingleOrDefault(m => m.Node.BaseUrl.ToLower() == baseUrl.ToLower());
            return servers.SingleOrDefault(m=>m.Node.BaseUrl.ToLower() == baseUrl.ToLower()).Node;            
        }

        public static List<INode> Get(NodeTypeEnum @enum)
        {
            return servers.Where(m => m.Node.NodeType == @enum).Select(m => m.Node).ToList();
        }
    }
}