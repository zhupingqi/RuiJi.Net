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
using RuiJi.Core;
using Newtonsoft.Json;

namespace RuiJi.Crawler
{
    public class CrawlerNodeService : ServiceBase
    {
        private static CrawlerNodeService _instance;


        public static CrawlerNodeService Instance
        {
            get
            {
                return _instance;
            }
        }

        static CrawlerNodeService()
        {
            _instance = new CrawlerNodeService();
        }

        private CrawlerNodeService()
        {
            
        }

        public string[] GetNodeIps()
        {
            if (zookeeper != null && zookeeper.State == ZooKeeper.States.CONNECTED)
            {
                var b = zookeeper.GetData("/config/" + BaseUrl + "/ips.txt", false, null);
                var r = System.Text.Encoding.UTF8.GetString(b);

                return r.Split('\n');
            }

            return new string[0];
        }

        public override void Start()
        {
            if (string.IsNullOrEmpty(BaseUrl) || string.IsNullOrEmpty(ZkServer))
                throw new Exception("before call start method must call setup method");

            ResetEvent = new ManualResetEvent(false);

            try
            {
                Console.WriteLine("crawler " + BaseUrl + " ready to startup!");
                Console.WriteLine("try connect to zookeeper server : " + ZkServer);

                zookeeper = new ZooKeeper(ZkServer, TimeSpan.FromSeconds(3), new SessionWatcher());
                ResetEvent.WaitOne();

                CreateNode();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CreateNode()
        {
            CreateCommonNode();

            var path = BaseUrl + "_crawler";

            var stat = zookeeper.Exists("/live_nodes/" + path, false);
            if (stat == null)
                zookeeper.Create("/live_nodes/" + path, null, Ids.READ_ACL_UNSAFE, CreateMode.Ephemeral);

            stat = zookeeper.Exists("/config/" + path, false);
            if (stat == null)
            {
                var d = new {
                    proxy = ProxyUrl,
                    ips = new string[0]
                };
                zookeeper.Create("/config/" + path, JsonConvert.SerializeObject(d).GetBytes(), Ids.READ_ACL_UNSAFE, CreateMode.Persistent);
            }
        }

        public override void Stop()
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
                            CrawlerNodeService.Instance.Start();
                            break;
                        }
                    case KeeperState.SyncConnected:
                        {
                            Console.WriteLine("zookeeper server connected!");
                            CrawlerNodeService.Instance.ResetEvent.Set();
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