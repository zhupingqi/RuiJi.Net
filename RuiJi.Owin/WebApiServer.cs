using Microsoft.Owin.Hosting;
using RuiJi.Core.Utils;
using RuiJi.Node;
using RuiJi.Node.Crawler;
using RuiJi.Node.CrawlerProxy;
using RuiJi.Node.Extracter;
using RuiJi.Node.ExtracterProxy;
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
        private IDisposable app;

        public string Port
        {
            get;
            private set;
        }

        public NodeBase NodeBase
        {
            get;
            internal set;
        }

        public void Start(string baseUrl, string nodeType, string zkServer, string proxy = "")
        {            
            this.Port = baseUrl.Split(':')[1];

            baseUrl = IPHelper.FixLocalUrl(baseUrl);

            app = WebApp.Start<Startup>("http://" + baseUrl);
            Console.WriteLine("Web Api Server Start At http://" + baseUrl + " with " + nodeType + " node");

            switch (nodeType)
            {
                case "c":
                    {
                        NodeBase = new CrawlerNode(baseUrl, zkServer, proxy);
                        break;
                    }
                case "cp":
                    {
                        NodeBase = new CrawlerProxyNode(baseUrl, zkServer);
                        break;
                    }
                case "e":
                    {
                        NodeBase = new ExtracterNode(baseUrl, zkServer, proxy);
                        break;
                    }
                case "ep":
                    {
                        NodeBase = new ExtracterProxyNode(baseUrl, zkServer);
                        break;
                    }
            }

            NodeBase.Start();
        }

        public void Stop()
        {
            if (app != null)
            {
                app.Dispose();
                app = null;
            }

            NodeBase.Stop();
        }
    }
}