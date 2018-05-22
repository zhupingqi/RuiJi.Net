using Microsoft.Owin.Hosting;
using RuiJi.Node;
using RuiJi.Node.Configuration;
using System;
using System.Collections.Generic;
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

        static ServerManager()
        {
            servers = new List<WebApiServer>();
            tasks = new List<Task>();
        }

        ~ServerManager()
        {
            StopAll();
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