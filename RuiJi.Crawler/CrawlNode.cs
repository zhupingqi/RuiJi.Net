using RuiJi.Core.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;
using System.Threading;

namespace RuiJi.Crawler
{
    public class CrawlNode
    {
        private static CrawlNode _instance;
        private ZooKeeper zookeeper;
        private string zkServer;
        private string baseUrl;
        private string clientIp;
        private string clientPort;

        protected ManualResetEvent restEvent;

        static CrawlNode()
        {
            _instance = new CrawlNode();
        }

        private CrawlNode()
        {
            LoadConfig();
        }

        public static CrawlNode Instance
        {
            get
            {
                return _instance;
            }
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

        public void Start()
        {
            restEvent = new ManualResetEvent(false);

            try
            {
                Console.WriteLine("crawler " + clientIp + " ready to startup!");
                Console.WriteLine("try connect to zookeeper server : " + zkServer);

                zookeeper = new ZooKeeper(zkServer, TimeSpan.FromSeconds(3), new SessionWatcher());
                restEvent.WaitOne();

                CreateNode();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CreateNode()
        {
            var stat = zookeeper.Exists("/live_nodes", false);
            if (stat == null)
                zookeeper.Create("/live_nodes", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zookeeper.Exists("/live_nodes/" + clientIp, false);
            if (stat == null)
                zookeeper.Create("/live_nodes/" + clientIp, null, Ids.READ_ACL_UNSAFE, CreateMode.Ephemeral);
        }

        public void Stop()
        {
            if (zookeeper != null)
            {
                zookeeper.Dispose();
                zookeeper = null;
            }
        }

        class SessionWatcher : IWatcher
        {
            public void Process(WatchedEvent @event)
            {
                if (@event.Type != EventType.None)
                    return;

                switch(@event.State)
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
                            CrawlNode.Instance.Start();
                            break;
                        }
                    case KeeperState.SyncConnected:
                        {
                            Console.WriteLine("zookeeper server connected!");
                            CrawlNode.Instance.restEvent.Set();
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
    }    
}