using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Host.HttpListener;

namespace RuiJi.Crawler
{
    public class CrawlerConsole
    {
        private static IDisposable app;

        public static void StartCrawler()
        {
            var baseUrl = ConfigurationManager.AppSettings.Get("baseUrl");

            using (app = WebApp.Start<Crawler.Startup>(baseUrl))
            {
                Console.WriteLine("Crawler Node Server Start At " + baseUrl);
                Process.Start(baseUrl);

                CrawlNode.Instance.Start();

                Console.ReadLine();
                CrawlNode.Instance.Stop();
            }

            StopCrawler();
        }

        public static void StopCrawler()
        {
            if(app!=null)
            {
                app.Dispose();
                app = null;
            }
        }
    }
}
