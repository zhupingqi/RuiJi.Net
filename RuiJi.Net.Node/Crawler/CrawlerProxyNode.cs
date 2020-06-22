using Newtonsoft.Json;
using org.apache.zookeeper;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Utils.Logging;
using System.Text;
using System.Threading.Tasks;
using static org.apache.zookeeper.Watcher.Event;
using static org.apache.zookeeper.ZooDefs;

namespace RuiJi.Net.Node.Crawler
{
    public class CrawlerProxyNode : NodeBase
    {
        public CrawlerProxyNode(string baseUrl, string zkServer) : base(baseUrl, zkServer)
        {
        }

        protected override void OnStartup()
        {
            base.CreateLiveNode("/live_nodes/proxy/" + BaseUrl, Encoding.UTF8.GetBytes("crawler proxy"));

            //create crawler proxy config in zookeeper
            var stat = zooKeeper.existsAsync("/config/proxy/" + BaseUrl, false).Result;
            if (stat == null)
            {
                var d = new
                {
                    type = "crawler",
                    mode = CrawlerProxyNodeModeEnum.MIX
                };

                zooKeeper.createAsync("/config/proxy/" + BaseUrl, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(d)), Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            }

            LoadLiveCrawler();
        }

        protected void LoadLiveCrawler()
        {
            CrawlerServerManager.Instance.Clear();

            try
            {
                var nodes = zooKeeper.getChildrenAsync("/live_nodes/crawler", new LiveCrawlerWatcher(this)).Result.Children;

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
                var b = zooKeeper.getDataAsync("/config/crawler/" + baseUrl, new CrawlerConfigWatcher(this)).Result.Data;
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

        class LiveCrawlerWatcher : Watcher
        {
            CrawlerProxyNode node;

            public LiveCrawlerWatcher(CrawlerProxyNode node)
            {
                this.node = node;
            }

            public override Task process(WatchedEvent @event)
            {
                switch (@event.get_Type())
                {
                    case EventType.NodeChildrenChanged:
                        {
                            node.LoadLiveCrawler();
                            Logger.GetLogger(node.BaseUrl).Info("detected crawler node change");
                            break;
                        }
                }

                return Task.CompletedTask;
            }
        }

        class CrawlerConfigWatcher : Watcher
        {
            CrawlerProxyNode node;

            public CrawlerConfigWatcher(CrawlerProxyNode node)
            {
                this.node = node;
            }

            public override Task process(WatchedEvent @event)
            {
                if (string.IsNullOrEmpty(@event.getPath()))
                    return Task.CompletedTask;

                var baseUrl = @event.getPath().Split('/')[3];

                switch (@event.get_Type())
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

                return Task.CompletedTask;
            }
        }
    }
}