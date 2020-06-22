using Newtonsoft.Json;
using org.apache.zookeeper;
using RuiJi.Net.Core.Utils.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static org.apache.zookeeper.Watcher.Event;
using static org.apache.zookeeper.ZooDefs;

namespace RuiJi.Net.Node.Feed
{
    public class FeedProxyNode : NodeBase
    {
        static FeedProxyNode()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LiteDb");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public FeedProxyNode(string baseUrl, string zkServer) : base(baseUrl, zkServer)
        {            
        }

        protected override void OnStartup()
        {
            base.CreateLiveNode("/live_nodes/proxy/" + BaseUrl, Encoding.UTF8.GetBytes("feed proxy"));

            //create crawler proxy config in zookeeper
            var stat = zooKeeper.existsAsync("/config/proxy/" + BaseUrl, false).Result;
            if (stat == null)
            {
                var d = new 
                {
                    type = "feed"
                };

                zooKeeper.createAsync("/config/proxy/" + BaseUrl, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(d)), Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            }

            LoadLiveFeed();
        }

        protected void LoadLiveFeed()
        {
            try
            {
                var nodes = zooKeeper.getChildrenAsync("/live_nodes/feed", new LiveFeedWatcher(this)).Result.Children;
                FeedManager.Instance.ClearAndAddServer(nodes.ToArray());
            }
            catch
            {
            }
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.FEEDPROXY;
        }

        class LiveFeedWatcher : Watcher
        {
            FeedProxyNode node;

            public LiveFeedWatcher(FeedProxyNode node)
            {
                this.node = node;
            }

            public override Task process(WatchedEvent @event)
            {
                switch (@event.get_Type())
                {
                    case EventType.NodeChildrenChanged:
                        {
                            node.LoadLiveFeed();
                            Logger.GetLogger(node.BaseUrl).Info("detected feed node change");
                            break;
                        }
                }

                return Task.CompletedTask;
            }
        }
    }
}