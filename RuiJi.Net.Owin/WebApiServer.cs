using log4net;
using Microsoft.Owin.Hosting;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Log;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Crawler;
using RuiJi.Net.Node.Extractor;
using RuiJi.Net.Node.Feed;
using RuiJi.Net.Node.Feed.LTS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.Owin
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

        public INode Node
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

            switch (nodeType)
            {
                case "c":
                    {
                        Node = new CrawlerNode(baseUrl, zkServer, proxy);
                        break;
                    }
                case "cp":
                    {
                        Node = new CrawlerProxyNode(baseUrl, zkServer);
                        break;
                    }
                case "e":
                    {
                        Node = new ExtractorNode(baseUrl, zkServer, proxy);
                        break;
                    }
                case "ep":
                    {
                        Node = new ExtractorProxyNode(baseUrl, zkServer);
                        break;
                    }
                case "f":
                    {
                        Node = new FeedNode(baseUrl, zkServer, proxy);
                        break;
                    }
                case "fp":
                    {
                        Node = new FeedProxyNode(baseUrl, zkServer);
                        break;
                    }
            }

            Node.Start();

            resetEvent = new ManualResetEvent(false);
            resetEvent.WaitOne();
        }

        public void StartStandalone(string baseUrl)
        {
            baseUrl = IPHelper.FixLocalUrl(baseUrl);

            app = WebApp.Start<Startup>("http://" + baseUrl);

            Node = new StandaloneNode(baseUrl);

            Node.Start();

            FeedScheduler.Start(baseUrl, "", null);
            FeedExtractScheduler.Start(baseUrl);
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

            Node.Stop();
            if (resetEvent != null)
                resetEvent.Set();

            Running = false;
        }
    }
}