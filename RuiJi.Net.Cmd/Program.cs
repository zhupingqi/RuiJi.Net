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

            while (true)
            {
                Console.WriteLine("please e to exit!");
                var key = Console.ReadLine();

                if (key != null && key.ToLower() == "e")
                {
                    ServerManager.StopAll();
                    break;
                }
            }

            Process.GetCurrentProcess().Kill();
        }
    }
}