using Microsoft.Owin.Hosting;
using RuiJi.Core.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Owin
{
    public class WebApiServer
    {
        private static IDisposable app;

        public void Start(string baseUrl,string nodeType,string zkServer)
        {
            app = WebApp.Start<Startup>(baseUrl);
            Console.WriteLine("Server Start At " + baseUrl);
        }

        public void Stop()
        {
            if (app != null)
            {
                app.Dispose();
                app = null;
            }
        }
    }
}