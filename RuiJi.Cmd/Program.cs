using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using System.Configuration;
using RuiJi.Crawler;
using RuiJi.Crawler.Proxy;

namespace RuiJi.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            StartupCrawlerProxy();
        }

        static void StartupCrawler()
        {
            var baseUrl = ConfigurationManager.AppSettings.Get("baseUrl");

            using (WebApp.Start<Crawler.Startup>(baseUrl))
            {
                Console.WriteLine("Web Server Start At " + baseUrl);
                Process.Start(baseUrl);

                CrawlNode.Instance.Start();

                Console.ReadLine();
                CrawlNode.Instance.Stop();
            }
        }

        static void StartupCrawlerProxy()
        {
            var baseUrl = ConfigurationManager.AppSettings.Get("baseUrl");

            using (WebApp.Start<Crawler.Proxy.Startup>(baseUrl))
            {
                Console.WriteLine("Web Server Start At " + baseUrl);
                Process.Start(baseUrl);

                ProxyNode.Instance.Start();

                Console.ReadLine();
                ProxyNode.Instance.Stop();
            }
        }
    }
}