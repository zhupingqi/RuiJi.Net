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
        private static List<Thread> threads;
        private static List<WebApiServer> servers;

        static ServerManager()
        {
            servers = new List<WebApiServer>();
            threads = new List<Thread>();
        }

        ~ServerManager()
        {
            StopAll();
        }

        public static void Start(string baseUrl, string type, string zkServer, string proxy = "")
        {
            var server = new WebApiServer();
            server.Start(baseUrl, type, zkServer, proxy);

            servers.Add(server);
        }

        public static void Stop(string port = "")
        {
            var server = servers.SingleOrDefault(m => m.Port == port);
            if (server != null)
            {
                server.Stop();
                servers.Remove(server);

                Console.WriteLine("server port with " + port + " stop!");
            }
        }

        internal static NodeBase GetNode(string port)
        {
            var server = servers.SingleOrDefault(m => m.Port == port);
            if (server != null)
            {
                return server.NodeBase;
            }

            return null;
        }

        public static void StartServers()
        {
            NodeConfigurationSection.Settings.ForEach(m =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        var t = new Thread(() =>
                        {
                            ServerManager.Start(m.BaseUrl, m.Type, m.ZkServer, m.Proxy);
                        });
                        t.Start();

                        threads.Add(t);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.WriteLine();
                });

                Thread.Sleep(1000);
            });
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
    }
}