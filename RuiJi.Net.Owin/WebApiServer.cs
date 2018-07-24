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
    public class WebApiServer : IServer
    {
        public IWebHost WebHost { get; private set; }

        public string BaseUrl { get; private set; }

        public string NodeType { get; private set; }

        public string ZkServer { get; private set; }

        public string Proxy { get; private set; }

        public int Port { get; private set; }

        public WebApiServer(string baseUrl, string nodeType, string zkServer = "", string proxy = "")
        {
            BaseUrl = IPHelper.FixLocalUrl(baseUrl);
            NodeType = nodeType;
            ZkServer = zkServer;
            Proxy = proxy;

            Port = 80;
            if (BaseUrl.IndexOf(":") != -1)
                Port = Convert.ToInt32(BaseUrl.Split(':')[1]);
        }

        public INode Node
        {
            get;
            internal set;
        }

        public void Start()
        {
            WebHost = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://" + BaseUrl)
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            WebHost.RunAsync();

            switch (NodeType)
            {
                case "c":
                    {
                        Node = new CrawlerNode(BaseUrl, ZkServer, Proxy);
                        break;
                    }
                case "cp":
                    {
                        Node = new CrawlerProxyNode(BaseUrl, ZkServer);
                        break;
                    }
                case "e":
                    {
                        Node = new ExtractorNode(BaseUrl, ZkServer, Proxy);
                        break;
                    }
                case "ep":
                    {
                        Node = new ExtractorProxyNode(BaseUrl, ZkServer);
                        break;
                    }
                case "f":
                    {
                        Node = new FeedNode(BaseUrl, ZkServer, Proxy);
                        break;
                    }
                case "fp":
                    {
                        Node = new FeedProxyNode(BaseUrl, ZkServer);
                        break;
                    }
                case "s":
                    {
                        Node = new StandaloneNode(this.BaseUrl);
                        break;
                    }
            }

            Node.Start();
        }

        public void Stop()
        {
            if (WebHost != null)
            {
                WebHost.StopAsync(TimeSpan.FromSeconds(0));
                WebHost.Dispose();
                WebHost = null;
            }

            Node.Stop();
        }
    }
}