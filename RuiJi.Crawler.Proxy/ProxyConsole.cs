using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Crawler.Proxy
{
    public class ProxyConsole
    {
        private static IDisposable app;

        public static void StartProxy()
        {
            var baseUrl = ConfigurationManager.AppSettings.Get("baseUrl");

            using (app = WebApp.Start<Proxy.Startup>(baseUrl))
            {
                Console.WriteLine("Proxy Server Start At " + baseUrl);
                Process.Start(baseUrl);

                ProxyNode.Instance.Start();

                Console.ReadLine();
                ProxyNode.Instance.Stop();
            }

            StopProxy();
        }

        public static void StopProxy()
        {
            if (app != null)
            {
                app.Dispose();
                app = null;
            }
        }
    }
}
