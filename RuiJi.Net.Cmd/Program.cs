using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core;
using RuiJi.Net.Owin;
using RuiJi.Net.Node.Crawler;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Extracter;
using System.Threading;
using RuiJi.Net.Core.Utils;
using log4net;
using RuiJi.Net.Node.Configuration;

namespace RuiJi.Net.Cmd
{
    public class Program
    {
        static List<Thread> threads;

        static Program()
        {
            threads = new List<Thread>();
        }

        ~Program()
        {

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