using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RuiJi.Crawler;
using RuiJi.Crawler.Proxy;

namespace RuiJi.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            //CrawlerConsole.StartCrawler();
            ProxyConsole.StartProxy();
        }

        ~Program()
        {
            CrawlerConsole.StopCrawler();
            ProxyConsole.StopProxy();
        }
    }
}