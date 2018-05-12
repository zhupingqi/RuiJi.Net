using RuiJi.Core;
using RuiJi.Core.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Crawler.Proxy
{
    public class ProxyNode
    {
        private static ProxyNode _instance;
        private ZooKeeper zookeeper;
        private string zkServer;
        private string baseUrl;
        private string clientIp;
        private string clientPort;

        protected ManualResetEvent restEvent;

        static ProxyNode()
        {
            _instance = new ProxyNode();
        }

        private ProxyNode()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            zkServer = ConfigurationManager.AppSettings.Get("zkServer");
            baseUrl = ConfigurationManager.AppSettings.Get("baseUrl");

            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                throw new ConfigurationErrorsException("baseUrl is not well formed!");
            }

            var u = new Uri(baseUrl);
            clientIp = u.Host.ToLower();
            clientPort = u.Port.ToString();

            if (!IPHelper.IsHostIPAddress(IPAddress.Parse(clientIp)))
            {
                throw new ConfigurationErrorsException("baseUrl is not allowed to use localhost or 127.0.0.1!");
            }
        }

        public static ProxyNode Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Start()
        {
            restEvent = new ManualResetEvent(false);

            try
            {
                Console.WriteLine("proxy " + clientIp + " ready to startup!");
                Console.WriteLine("try connect to zookeeper server : " + zkServer);

                zookeeper = new ZooKeeper(zkServer, TimeSpan.FromSeconds(3), new SessionWatcher());
                restEvent.WaitOne();

                CreateNode();
                LoadLiveNodes();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            zookeeper.Dispose();
            zookeeper = null;
        }

        private void CreateNode()
        {
            var stat = zookeeper.Exists("/live_nodes", false);
            if (stat == null)
                zookeeper.Create("/live_nodes", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            stat = zookeeper.Exists("/config", false);
            if (stat == null)
                zookeeper.Create("/config", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
        }

        public void LoadLiveNodes()
        {
            CrawlerManager.Instance.Clear();

            var lives = zookeeper.GetChildren("/live_nodes", false);

            foreach (var ip in lives)
            {
                var ips = GetCrawlerIps(ip);
                CrawlerManager.Instance.AddServer(ip,ips);
            }
        }

        public string[] GetCrawlerIps(string clientIp)
        {
            var b = zookeeper.GetData("/config/" + clientIp + "/ips.txt", false, null);
            var r = System.Text.Encoding.UTF8.GetString(b);

            return r.Split('\n');
        }

        class SessionWatcher : IWatcher
        {
            public void Process(WatchedEvent @event)
            {
                if (@event.Type != EventType.None)
                {
                    var segments = @event.Path.TrimStart('/').Split('/');

                    switch(segments[0])
                    {
                        case "live_nodes":
                            {
                                ProcessLiveNodes(@event, segments);
                                break;
                            }
                        case "config":
                            {
                                ProcessConfig(@event, segments);
                                break;
                            }
                    }
                }
                else
                {
                    switch (@event.State)
                    {
                        case KeeperState.Disconnected:
                            {
                                Console.WriteLine("disconnected with zookeeper server");
                                //CrawlNode.Instance.Start();
                                break;
                            }
                        case KeeperState.Expired:
                            {
                                Console.WriteLine("connected expired! reconnect!");
                                ProxyNode.Instance.Start();
                                break;
                            }
                        case KeeperState.SyncConnected:
                            {
                                Console.WriteLine("zookeeper server connected!");
                                ProxyNode.Instance.restEvent.Set();
                                break;
                            }
                        case KeeperState.NoSyncConnected:
                            {
                                Console.WriteLine("zookeeper server NoSyncConnected!");
                                break;
                            }
                        case KeeperState.Unknown:
                            {
                                break;
                            }
                    }
                }
            }

            private void ProcessLiveNodes(WatchedEvent @event, string[] segments)
            {
                var clientIp = segments[1];

                switch (@event.Type)
                {
                    case EventType.NodeCreated:
                        {
                            var ips = ProxyNode.Instance.GetCrawlerIps(clientIp);
                            CrawlerManager.Instance.AddServer(clientIp, ips);
                            break;
                        }
                    case EventType.NodeDeleted:
                        {
                            CrawlerManager.Instance.RemoveServer(clientIp);
                            break;
                        }
                }
            }

            private void ProcessConfig(WatchedEvent @event, string[] segments)
            {
                if (segments.Length == 3)
                {
                    var clientIp = segments[1];

                    switch (@event.Type)
                    {
                        case EventType.NodeCreated:
                        case EventType.NodeDataChanged:
                            {
                                if(segments[2] == "ips.txt")
                                {
                                    var ips = ProxyNode.Instance.GetCrawlerIps(clientIp);
                                    CrawlerManager.Instance.AddServer(clientIp, ips);
                                }
                                
                                break;
                            }
                        case EventType.NodeDeleted:
                            {
                                break;
                            }
                    }
                }
            }
        }
    }
}