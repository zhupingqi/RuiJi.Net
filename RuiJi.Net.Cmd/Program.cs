using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Owin;
using System;

namespace RuiJi.Net.Cmd
{
    class Program
    {
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
