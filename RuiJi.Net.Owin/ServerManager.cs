using RuiJi.Net.Core.Code.Compiler;
using RuiJi.Net.Core.Code.Provider;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Compile;
using RuiJi.Net.Node.Feed.Db;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace RuiJi.Net.Owin
{
    public class ServerManager
    {
        private static List<IServer> servers;
        private static Process zkProcess;

        static ServerManager()
        {
            servers = new List<IServer>();
        }

        ~ServerManager()
        {
            StopAll();
            if (zkProcess != null)
                zkProcess.Kill();
        }

        public static void Start(string baseUrl, string type, string zkServer = "", string proxy = "")
        {           
            var server = new WebApiServer(baseUrl, type, zkServer, proxy);
            servers.Add(server);

            server.Start();
        }

        public static void Stop(int port = 80)
        {
            var server = servers.SingleOrDefault(m => m.Port == port);
            if (server != null)
            {
                server.Stop();

                Logger.GetLogger("").Info("server with " + server.BaseUrl + " stop!");
            }
        }

        public static void StartServers()
        {
            if (!String.IsNullOrEmpty(RuiJiConfiguration.DocServer))
            {
                var server = new DocumentServer(RuiJiConfiguration.DocServer);
                server.Start();

                servers.Add(server);
            }

            if (RuiJiConfiguration.Standalone)
            {
                var baseUrl = RuiJiConfiguration.RuiJiServer;
                if (string.IsNullOrEmpty(baseUrl))
                {
                    Logger.GetLogger("").Info("RuiJiServer not exsit in AppSettings");
                    return;
                }

                try
                {
                    Start(baseUrl, "s");
                }
                catch (Exception ex)
                {
                    Logger.GetLogger("").Fatal(ex.Message);
                }

                CodeCompilerManager.Create("url", new List<ICodeProvider> {
                    new LiteDbCodeProvider(Node.Feed.Db.FuncType.URLFUNCTION)
                });

                CodeCompilerManager.Create("proc", new List<ICodeProvider> {
                    new LiteDbCodeProvider(Node.Feed.Db.FuncType.SELECTORPROCESSOR)
                });
            }
            else
            {
                var zkServer = RuiJiConfiguration.ZkServer;
                if (string.IsNullOrEmpty(zkServer))
                {
                    Logger.GetLogger("").Info("zkServer not defined");
                    return;
                }

                StartZKServer();

                RuiJiConfiguration.Nodes.ForEach(m =>
                {
                    try
                    {
                        Start(m.BaseUrl, m.Type, zkServer, m.Proxy);
                    }
                    catch (Exception ex)
                    {
                        Logger.GetLogger("").Info(ex.Message);
                    }
                });

                CodeCompilerManager.Create("url", new List<ICodeProvider> {
                    new RemoteCodeProvider(ZkNode().BaseUrl,FuncType.URLFUNCTION.ToString())
                });

                CodeCompilerManager.Create("proc", new List<ICodeProvider> {
                    new RemoteCodeProvider(ZkNode().BaseUrl,FuncType.SELECTORPROCESSOR.ToString())
                });
            }
        }

        public static void StartZKServer()
        {
            var zkPath = RuiJiConfiguration.ZkPath;
            if (!string.IsNullOrEmpty(zkPath))
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, zkPath,"bin", "zkServer.cmd");

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
        }

        public static void Start(int port)
        {
            var server = servers.SingleOrDefault(m => m.Port == port);
            if (server != null)
            {
                if (server.WebHost != null)
                {
                    Logger.GetLogger("").Info("server " + server.Node.BaseUrl + " already running!");
                }
                else
                {
                    try
                    {
                        server.Start();
                    }
                    catch (Exception ex)
                    {
                        Logger.GetLogger("").Info(ex.Message);
                    }

                    Logger.GetLogger("").Info("server " + server.Node.BaseUrl + " restart!");
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
                    Logger.GetLogger("").Info("server port with " + m.BaseUrl + " stop!");
                });
                servers.Clear();
            }
            catch (Exception ex)
            {
                Logger.GetLogger("").Info(ex.Message);
            }

            Logger.GetLogger("").Info("all server stop!");
        }

        public static NodeBase ZkNode()
        {
            var server = servers.SingleOrDefault(m => m.Node.IsLeader);
            if (server != null)
                return (NodeBase)server.Node;

            return (NodeBase)servers.First().Node;
        }

        public static INode Get(string baseUrl)
        {
            //aliyun nat...
            baseUrl = baseUrl.Replace("118.31.61.230", "172.16.50.52");

            var ss = servers.Where(m => m.Node != null).ToList();

            var temp = ss.SingleOrDefault(m => m.Node.BaseUrl.ToLower() == baseUrl.ToLower());
            return ss.SingleOrDefault(m => m.Node.BaseUrl.ToLower() == baseUrl.ToLower()).Node;
        }

        public static List<INode> Get(NodeTypeEnum @enum)
        {
            return servers.Where(m => m.Node.NodeType == @enum).Select(m => m.Node).ToList();
        }
    }
}