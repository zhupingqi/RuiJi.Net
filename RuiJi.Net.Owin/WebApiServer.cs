using Microsoft.AspNetCore.Hosting;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Crawler;
using RuiJi.Net.Node.Extractor;
using RuiJi.Net.Node.Feed;
using System;
using System.IO;
using System.Threading;

namespace RuiJi.Net.Owin
{
    public class WebApiServer
    {
        private IWebHost webHost;

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

            webHost = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://" + baseUrl)
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            webHost.RunAsync();

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
            this.baseUrl = IPHelper.FixLocalUrl(baseUrl);

            webHost = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://" + this.baseUrl)
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            webHost.RunAsync();

            Node = new StandaloneNode(this.baseUrl);

            Node.Start();
        }

        public void Restart()
        {
            Start(baseUrl, nodeType, zkServer, proxy);
        }

        public void Stop()
        {
            if (webHost != null)
            {
                webHost.StopAsync(TimeSpan.FromSeconds(0));
                webHost.Dispose();
                webHost = null;
            }

            Node.Stop();
            if (resetEvent != null)
                resetEvent.Set();

            Running = false;
        }
    }
}