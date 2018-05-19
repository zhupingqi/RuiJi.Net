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

            StartServers();

            while (true)
            {
                var cmd = Console.ReadLine();
                if (cmd == "quit")
                {
                    ServerManager.Inst.Stop();
                    break;
                }
            }
        }

        public static void StartServers()
        {
            NodeConfigurationSection.Settings.ForEach(m =>
            {
                Task.Run(() =>
                {
                    StartServer(m);
                    Console.WriteLine();
                });

                Thread.Sleep(1000);
            });
        }

        static void StartServer(NodeConfigurationElement node)
        {
            try
            {
                var t = new Thread(() =>
                {
                    ServerManager.Inst.Start(node);
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