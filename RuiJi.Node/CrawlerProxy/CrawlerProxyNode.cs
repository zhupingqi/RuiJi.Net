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
            var stat = ZooKeeper.Exists("/live_nodes/proxy/" + BaseUrl, false);
            if (stat == null)
                ZooKeeper.Create("/live_nodes/proxy/" + BaseUrl, "crawler proxy".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);

            //create crawler proxy config in zookeeper
            stat = ZooKeeper.Exists("/config/proxy/" + BaseUrl, false);
            if (stat == null)
            {
                var d = new
                {
                    type = "crawler",
                    mode = CrawlerProxyNodeModeEnum.MIX
                };

                ZooKeeper.Create("/config/proxy/" + BaseUrl, JsonConvert.SerializeObject(d).GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }

            LoadNodes();
        }

        private void LoadNodes()
        {
            CrawlerManager.Instance.Clear();

            var nodes = ZooKeeper.GetChildren("/live_nodes/crawler", false);

            foreach (var node in nodes)
            {
                var d = GetCrawlerConfig(node);
                if (d.Proxy == BaseUrl)
                    CrawlerManager.Instance.AddServer(node, d.Ips);
            }
        }

        public CrawlerConfig GetCrawlerConfig(string baseUrl)
        {
            var b = ZooKeeper.GetData("/config/crawler/" + baseUrl, false, null);
            var r = System.Text.Encoding.UTF8.GetString(b);
            var d = JsonConvert.DeserializeObject<CrawlerConfig>(r);

            return d;
        }

        protected override void Process(WatchedEvent @event)
        {
            if (@event.Type != EventType.None)
            {
                var segments = @event.Path.TrimStart('/').Split('/');

                switch (segments[0])
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
        }

        private void ProcessLiveNodes(WatchedEvent @event, string[] segments)
        {
            if (segments.Length == 3)
            {
                var baseUrl = segments[2];

                switch (@event.Type)
                {
                    case EventType.NodeCreated:
                        {
                            var d = GetCrawlerConfig(baseUrl);
                            CrawlerManager.Instance.AddServer(baseUrl, d.Ips);
                            break;
                        }
                    case EventType.NodeDeleted:
                        {
                            CrawlerManager.Instance.RemoveServer(baseUrl);
                            break;
                        }
                }
            }
        }

        private void ProcessConfig(WatchedEvent @event, string[] segments)
        {
            if (segments.Length == 3)
            {
                var baseUrl = segments[2];

                switch (@event.Type)
                {
                    case EventType.NodeDataChanged:
                        {
                            var d = GetCrawlerConfig(baseUrl);
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