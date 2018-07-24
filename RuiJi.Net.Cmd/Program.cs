using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Owin;
using System;
using System.Diagnostics;

namespace RuiJi.Net.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.GetLogger("").Info("Program started!");

            ServerManager.StartServers();

            //while (true)
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("please type stop port/all, quit or start port");
            //    var cmd = Console.ReadLine();
            //    if (cmd == "quit")
            //    {
            //        Logger.GetLogger("").Info("Program quit!");
            //        break;
            //    }

            //    if (cmd != null && cmd.StartsWith("stop"))
            //    {
            //        var sp = cmd.Split(' ');
            //        if (sp.Length == 2)
            //        {
            //            var port = sp[1];
            //            if (port == "all")
            //            {
            //                ServerManager.StopAll();
            //                Logger.GetLogger("").Info("Program stop all!");
            //            }
            //            else
            //            {
            //                int p;
            //                if (Int32.TryParse(port, out p))
            //                {
            //                    ServerManager.Stop(p);

            //                    Logger.GetLogger("").Info("Program stop " + port + "!");
            //                }
            //                else
            //                {
            //                    Logger.GetLogger("").Info("Unavailable " + port + "!");
            //                }
            //            }
            //        }
            //        else
            //        {
            //            Console.WriteLine("type stop port/stop all");
            //        }
            //    }

            //    if (cmd != null && cmd.StartsWith("start"))
            //    {
            //        var port = cmd.Split(' ')[1];
            //        ServerManager.Start(Convert.ToInt32(port));
            //    }
            //}

            while (true)
            {
                Console.WriteLine("please ctrl + e to exit!");
                var key = Console.ReadKey(true);

                if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key.ToString().ToLower() == "e")
                {
                    ServerManager.StopAll();
                    break;
                }
            }

            Process.GetCurrentProcess().Kill();
        }
    }
}