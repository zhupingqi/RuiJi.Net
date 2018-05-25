using Microsoft.Owin.Hosting;
using RuiJi.Core.Utils;
using RuiJi.Node;
using RuiJi.Node.Crawler;
using RuiJi.Node.Extracter;
using RuiJi.Node.Feed;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Owin
{
    public class WebApiServer
    {
        private IDisposable app;

        private ManualResetEvent resetEvent;

        public bool Running
        {
            get;
            private set;
        }

        private string baseUrl;

        private string nodeType;

        private string zkServer;

        private string proxy;

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
            Running = true;

            this.Port = baseUrl.Split(':')[1];

            this.baseUrl = baseUrl;
            this.nodeType = nodeType;
            this.zkServer = zkServer;
            this.proxy = proxy;

            baseUrl = IPHelper.FixLocalUrl(baseUrl);

            app = WebApp.Start<Startup>("http://" + baseUrl);
            Console.WriteLine("Web Api Server Start At http://" + baseUrl + " with " + nodeType + " node");
            Console.WriteLine();

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
                case "f":
                    {
                        NodeBase = new FeedNode(baseUrl, zkServer, proxy);
                        break;
                    }
                case "fp":
                    {
                        NodeBase = new FeedProxyNode(baseUrl, zkServer);
                        break;
                    }
            }

            NodeBase.Start();

            resetEvent = new ManualResetEvent(false);
            resetEvent.WaitOne();
        }

        public void Restart()
        {
            Start(baseUrl, nodeType, zkServer, proxy);
        }

        public void Stop()
        {
            if (app != null)
            {
                app.Dispose();
                app = null;
            }

            NodeBase.Stop();
            if(resetEvent != null)
                resetEvent.Set();

            Running = false;
        }
    }
}