using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace RuiJi.Crawler.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseUrl = System.Configuration.ConfigurationManager.AppSettings.Get("baseUrl");
            //AppDomain.CurrentDomain.Load(typeof(Microsoft.Owin.Host.HttpListener.OwinHttpListener).Assembly.GetName());
            //启动监听
            using (WebApp.Start<Startup>(baseUrl))
            {
                Console.WriteLine("Server Start!");
                Process.Start(baseUrl);
                Console.ReadLine();
            }
        }
    }
}