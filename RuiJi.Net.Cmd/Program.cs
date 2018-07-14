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
using RuiJi.Net.Node.Extractor;
using System.Threading;
using RuiJi.Net.Core.Utils;
using log4net;
using RuiJi.Net.Core.Utils.Logging;

namespace RuiJi.Net.Cmd
{
    public class Program
    {
        static Program()
        {
            Logger.Add("", new List<IAppender> {
                new RollingFileAppender(""),
                new ConsoleAppender()
            });
        }

        ~Program()
        {

        }

        static void Main(string[] args)
        {
            Logger.GetLogger("").Info("Program started!");

            ServerManager.StartServers();

            ServerManager.StartDocServer();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("please type stop port/all, quit or start port");
                var cmd = Console.ReadLine();
                if (cmd == "quit")
                {
                    Logger.GetLogger("").Info("Program quit!");
                    break;
                }

                if (cmd.StartsWith("stop"))
                {
                    var sp = cmd.Split(' ');
                    if (sp.Length == 2)
                    {
                        var port = sp[1];
                        if (port == "all")
                        {
                            ServerManager.StopAll();
                            Logger.GetLogger("").Info("Program stop all!");
                        }
                        else
                        {
                            ServerManager.Stop(port);
                            Logger.GetLogger("").Info("Program stop " + port + "!");
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
                    ServerManager.Start(Convert.ToInt32(port));
                }
            }

            ServerManager.StopAll();

            Logger.GetLogger("").Info("Program stop all!");
        }
    }
}