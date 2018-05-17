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
using CommandLine.Text;
using CommandLine;
using RuiJi.Node.Crawler;
using RuiJi.Node.CrawlerProxy;
using RuiJi.Node;
using RuiJi.Node.Extracter;
using RuiJi.Node.ExtracterProxy;
using System.Threading;
using RuiJi.Core.Utils;

namespace RuiJi.Cmd
{
    public class Program
    {
        static WebApiServer _server;

        static Program()
        {
            
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("please input args");
                return;
            }

            if (args[0] == "start")
            {
                try
                {
                    Parser.Default.ParseArguments<NodeStartOptions>(args).WithParsed(opt =>
                    {

                        _server = new WebApiServer();
                        var baseUrl = IPHelper.FixLocalUrl(opt.BaseUrl);
                        _server.Start("http://" + baseUrl, opt.Type, opt.ZkServer);
                        //Process.Start("http://" + opt.BaseUrl);

                        //need run in thread
                        NodeBase serviceBase = null;
                        switch (opt.Type)
                        {
                            case "c":
                                {
                                    serviceBase = new CrawlerNode(opt.BaseUrl, opt.ZkServer, opt.Proxy);
                                    break;
                                }
                            case "cp":
                                {
                                    serviceBase = new CrawlerProxyNode(opt.BaseUrl, opt.ZkServer);
                                    break;
                                }
                            case "e":
                                {
                                    serviceBase = new ExtracterNode(opt.BaseUrl, opt.ZkServer, opt.Proxy);
                                    break;
                                }
                            case "ep":
                                {
                                    serviceBase = new ExtracterProxyNode(opt.BaseUrl, opt.ZkServer);
                                    break;
                                }
                            case "f":
                                {
                                    break;
                                }
                        }

                        if (serviceBase != null)
                        {
                            serviceBase.Start();
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (args[0] == "stop")
            {
                if (args[1] == "all")
                {

                }
                else
                {
                    try
                    {
                        var opts = Parser.Default.ParseArguments<NodeStopOptions>(args);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            Console.ReadKey(true);
            //Application.Run();
            //new ManualResetEvent(false).WaitOne();
        }

        ~Program()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }
        }

        class NodeStartOptions
        {
            [Option('u', "url", Required = true, HelpText = "node url, e.g. 192.168.0.2:39001")]
            public string BaseUrl { get; set; }

            [Option('t', "type", Required = true, HelpText = "node type, e.g. c[crawler]/cp[crawler proxy]/e[extracter]/ep[extracter proxy]")]
            public string Type { get; set; }

            [Option('p', "proxy", HelpText = "proxy url, e.g. e.g. 192.168.0.2:39000")]
            public string Proxy { get; set; }

            [Option('z', "zkServer", Required = true, HelpText = "zookeeper url, e.g. 192.168.0.2:2181")]
            public string ZkServer { get; set; }
        }

        class NodeStopOptions
        {
            [Option('p', "port", HelpText = "stop node with port")]
            public string Port { get; set; }
        }
    }
}