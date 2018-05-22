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
        static List<Thread> threads;

        static Program()
        {
            threads = new List<Thread>();
        }

        static void Main(string[] args)
        {
            LogManager.GetCurrentLoggers().First().Info("Program started!");

            ServerManager.StartServers();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("please type stop port/all, quit or start port");
                var cmd = Console.ReadLine();
                if (cmd == "quit")
                {
                    break;
                }

                if (cmd.StartsWith("stop"))
                {
                    var sp = cmd.Split(' ');
                    if (sp.Length == 2)
                    {
                        var port = sp[1];
                        if (port == "all")
                            ServerManager.StopAll();
                        else
                        {
                            ServerManager.Stop(port);
                        }
                    }
                    else
                    {
                        Console.WriteLine("type stop port/stop all");
                    }
                }

                if (cmd.StartsWith("start"))
                {
                    var port = cmd.Split(' ')[1];
                    ServerManager.Start(port);
                }
            }

            ServerManager.StopAll();
        }
    }
}