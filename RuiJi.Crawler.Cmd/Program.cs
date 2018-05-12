using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using System.Configuration;

namespace RuiJi.Crawler.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseUrl = ConfigurationManager.AppSettings.Get("baseUrl");

            using (WebApp.Start<Startup>(baseUrl))
            {
                Console.WriteLine("Server Start!");

                CrawlNode.Instance.Start();
                Process.Start(baseUrl);

                Console.ReadLine();
                CrawlNode.Instance.Stop();
            }
        }
    }
}