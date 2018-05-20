using Newtonsoft.Json;
using RuiJi.Core;
using RuiJi.Core.Utils;
using RuiJi.Node.Crawler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Node.CrawlerProxy
{
    public class CrawlerProxyNode : NodeBase
    {

        public CrawlerProxyNode(string baseUrl, string zkServer) : base(baseUrl, zkServer)
        {

        }

        protected override void OnStartup()
        {
            var stat = zooKeeper.Exists("/live_nodes/proxy/" + BaseUrl, false);
            if (stat == null)
                zooKeeper.Create("/live_nodes/proxy/" + BaseUrl, "crawler proxy".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);

            //create crawler proxy config in zookeeper
            stat = zooKeeper.Exists("/config/proxy/" + BaseUrl, false);
            if (stat == null)
            {
                var d = new
                {
                    type = "crawler",
                    mode = CrawlerProxyNodeModeEnum.MIX
                };

                zooKeeper.Create("/config/proxy/" + BaseUrl, JsonConvert.SerializeObject(d).GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }

            LoadLiveCrawler();
        }

        protected void LoadLiveCrawler()
        {
            CrawlerManager.Instance.Clear();

            var nodes = zooKeeper.GetChildren("/live_nodes/crawler", new LiveCrawlerWatcher(this));

            foreach (var node in nodes)
            {
                var d = GetCrawlerConfig(node);
                if (d.Proxy == BaseUrl)
                    CrawlerManager.Instance.AddServer(node, d.Ips);
            }
        }

        public CrawlerConfig GetCrawlerConfig(string baseUrl)
        {
            var b = zooKeeper.GetData("/config/crawler/" + baseUrl, new CrawlerConfigWatcher(this), null);
            var r = System.Text.Encoding.UTF8.GetString(b);
            var d = JsonConvert.DeserializeObject<CrawlerConfig>(r);

            return d;
        }

        class LiveCrawlerWatcher : IWatcher
        {
            CrawlerProxyNode node;

            public LiveCrawlerWatcher(CrawlerProxyNode node)
            {
                this.node = node;
            }

            public void Process(WatchedEvent @event)
            {
                switch (@event.Type)
                {
                    case EventType.NodeChildrenChanged:
                        {
                            node.LoadLiveCrawler();
                            Console.WriteLine("detected crawler node change");
                            break;
                        }
                }
            }
        }

        class CrawlerConfigWatcher : IWatcher
        {
            CrawlerProxyNode node;

            public CrawlerConfigWatcher(CrawlerProxyNode node)
            {
                this.node = node;
            }

            public void Process(WatchedEvent @event)
            {
                if (string.IsNullOrEmpty(@event.Path))
                    return;

                var baseUrl = @event.Path.Split('/')[2];

                switch (@event.Type)
                {
                    case EventType.NodeDataChanged:
                        {
                            var d = node.GetCrawlerConfig(baseUrl);
                            CrawlerManager.Instance.AddServer(baseUrl, d.Ips);

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