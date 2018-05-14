using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RuiJi.Crawler;
using RuiJi.Crawler.Proxy;
using RuiJi.Core;
using RuiJi.Owin;
using CommandLine.Text;
using CommandLine;

namespace RuiJi.Cmd
{
    class Program
    {
        static WebApiServer _server;

        static Program()
        {

        }

        static void Main(string[] args)
        {
            args = new string[] {
                "start",
                "-u",
                "192.168.31.196:39001",
                "-t",
                "c",
                "-p",
                "192.168.31.196:39000",
                "-z",
                "192.168.31.196:2181"
            };

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
                        _server.Start("http://" + opt.BaseUrl, opt.Type, opt.ZkServer);
                        Process.Start("http://" + opt.BaseUrl);

                        //need run in thread
                        switch (opt.Type)
                        {
                            case "c":
                                {
                                    if (string.IsNullOrEmpty(opt.Proxy))
                                        Console.WriteLine("need proxy argment");
                                    else
                                    {
                                        CrawlerNodeService.Instance.Setup(opt.BaseUrl, opt.ZkServer, opt.Proxy);
                                        CrawlerNodeService.Instance.Start();
                                    }
                                    break;
                                }
                            case "cp":
                                {
                                    CrawlerProxyNodeService.Instance.Setup(opt.BaseUrl, opt.ZkServer);
                                    CrawlerProxyNodeService.Instance.Start();
                                    break;
                                }
                            case "e":
                                {
                                    break;
                                }
                            case "ep":
                                {
                                    break;
                                }
                            case "f":
                                {
                                    break;
                                }
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

            Console.ReadLine();
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