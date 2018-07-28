using Newtonsoft.Json;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Node.Crawler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Net.Node.Crawler
{
    public class CrawlerProxyNode : NodeBase
    {
        public CrawlerProxyNode(string baseUrl, string zkServer) : base(baseUrl, zkServer)
        {
        }

        protected override void OnStartup()
        {
            base.CreateLiveNode("/live_nodes/proxy/" + BaseUrl, "crawler proxy".GetBytes());

            //create crawler proxy config in zookeeper
            var stat = zooKeeper.Exists("/config/proxy/" + BaseUrl, false);
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
            CrawlerServerManager.Instance.Clear();

            try
            {
                var nodes = zooKeeper.GetChildren("/live_nodes/crawler", new LiveCrawlerWatcher(this));

                foreach (var node in nodes)
                {
                    var d = GetCrawlerConfig(node);
                    if (d.Proxy == BaseUrl)
                        CrawlerServerManager.Instance.AddServer(node, d.Ips);
                }
            }
            catch
            {
            }
        }

        public CrawlerConfig GetCrawlerConfig(string baseUrl)
        {
            try
            {
                var b = zooKeeper.GetData("/config/crawler/" + baseUrl, new CrawlerConfigWatcher(this), null);
                var r = Encoding.UTF8.GetString(b);
                var d = JsonConvert.DeserializeObject<CrawlerConfig>(r);

                return d;
            }
            catch
            {
            }

            return null;
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.CRAWLERPROXY;
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
                            Logger.GetLogger(node.BaseUrl).Info("detected crawler node change");
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

                var baseUrl = @event.Path.Split('/')[3];

                switch (@event.Type)
                {
                    case EventType.NodeDataChanged:
                        {
                            var d = node.GetCrawlerConfig(baseUrl);
                            CrawlerServerManager.Instance.AddServer(baseUrl, d.Ips);

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