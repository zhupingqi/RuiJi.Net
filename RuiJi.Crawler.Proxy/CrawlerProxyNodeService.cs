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
    public class CrawlerProxyNodeService : ServiceBase
    {
        private static CrawlerProxyNodeService _instance;

        static CrawlerProxyNodeService()
        {
            _instance = new CrawlerProxyNodeService();
        }

        private CrawlerProxyNodeService()
        {
            
        }

        public static CrawlerProxyNodeService Instance
        {
            get
            {
                return _instance;
            }
        }

        public override void Start()
        {
            ResetEvent = new ManualResetEvent(false);

            try
            {
                Console.WriteLine("proxy " + BaseUrl + " ready to startup!");
                Console.WriteLine("try connect to zookeeper server : " + ZkServer);

                zookeeper = new ZooKeeper(ZkServer, TimeSpan.FromSeconds(3), new SessionWatcher());
                ResetEvent.WaitOne();

                CreateNode();
                LoadLiveNodes();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void Stop()
        {
            zookeeper.Dispose();
            zookeeper = null;
        }

        private void CreateNode()
        {
            CreateCommonNode();

            var path = BaseUrl + "_crawlerproxy";

            var stat = zookeeper.Exists("/live_nodes/" + path, false);
            if (stat == null)
                zookeeper.Create("/live_nodes/" + path, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);

            stat = zookeeper.Exists("/config/" + path, false);
            if (stat == null)
            {
                zookeeper.Create("/config/" + path, null, Ids.READ_ACL_UNSAFE, CreateMode.Persistent);
            }
        }

        public void LoadLiveNodes()
        {
            CrawlerManager.Instance.Clear();

            var nodes = zookeeper.GetChildren("/live_nodes", false);

            foreach (var node in nodes)
            {
                if (node.EndsWith("_crawler"))
                {
                    var ip = node.Split('/')[1].Replace("_crawler","");
                    var ips = GetCrawlerIps(ip);
                    CrawlerManager.Instance.AddServer(ip, ips);
                }
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
                                CrawlerProxyNodeService.Instance.Start();
                                break;
                            }
                        case KeeperState.SyncConnected:
                            {
                                Console.WriteLine("zookeeper server connected!");
                                CrawlerProxyNodeService.Instance.ResetEvent.Set();
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
                            var ips = CrawlerProxyNodeService.Instance.GetCrawlerIps(clientIp);
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
                                    var ips = CrawlerProxyNodeService.Instance.GetCrawlerIps(clientIp);
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