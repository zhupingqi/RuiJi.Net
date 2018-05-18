using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RuiJi.Core.Crawler;
using RuiJi.Core;
using RuiJi.Owin;
using RuiJi.Node.Crawler;
using RuiJi.Node.CrawlerProxy;
using RuiJi.Node;
using RuiJi.Node.Extracter;
using RuiJi.Node.ExtracterProxy;
using System.Threading;
using RuiJi.Core.Utils;
using log4net;
using RuiJi.Node.Configuration;

namespace RuiJi.Cmd
{
    public class Program
    {
        static List<WebApiServer> servers;
        static List<Thread> threads;

        static Program()
        {
            servers = new List<WebApiServer>();
            threads = new List<Thread>();
        }

        static void Main(string[] args)
        {
            LogManager.GetCurrentLoggers().First().Info("Program started!");

            StartNodes();

            while (true)
            {
                var cmd = Console.ReadLine();
                if (cmd == "quit")
                    break;
            }
        }

        ~Program()
        {
            foreach (var server in servers)
            {
                if (server != null)
                {
                    server.Stop();
                }
            }
        }

        public static void StartNodes()
        {
            NodeConfigurationSection.Settings.ForEach(m =>
            {
                Task.Run(() =>
                {
                    StartNode(m);
                });

                Thread.Sleep(1000);
            });
        }

        static void StartNode(NodeConfigurationElement node)
        {
            try
            {
                var t = new Thread(() =>
                {
                    NodeBase serviceBase = null;
                    switch (node.Type)
                    {
                        case "c":
                            {
                                serviceBase = new CrawlerNode(node.BaseUrl, node.ZkServer, node.Proxy);
                                break;
                            }
                        case "cp":
                            {
                                serviceBase = new CrawlerProxyNode(node.BaseUrl, node.ZkServer);
                                break;
                            }
                        case "e":
                            {
                                serviceBase = new ExtracterNode(node.BaseUrl, node.ZkServer, node.Proxy);
                                break;
                            }
                        case "ep":
                            {
                                serviceBase = new ExtracterProxyNode(node.BaseUrl, node.ZkServer);
                                break;
                            }
                    }

                    serviceBase.Start();

                    var server = new WebApiServer();

                    var baseUrl = IPHelper.FixLocalUrl(node.BaseUrl);
                    server.Start(serviceBase, "http://" + baseUrl, node.Type, node.ZkServer);

                    servers.Add(server);

                    Console.ReadLine();
                });
                t.Start();

                threads.Add(t);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}