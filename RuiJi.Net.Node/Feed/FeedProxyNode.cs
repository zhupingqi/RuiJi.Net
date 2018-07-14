using Newtonsoft.Json;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Node.Extractor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

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
            base.CreateLiveNode("/live_nodes/proxy/" + BaseUrl, "feed proxy".GetBytes());

            //create crawler proxy config in zookeeper
            var stat = zooKeeper.Exists("/config/proxy/" + BaseUrl, false);
            if (stat == null)
            {
                var d = new 
                {
                    type = "feed"
                };

                zooKeeper.Create("/config/proxy/" + BaseUrl, JsonConvert.SerializeObject(d).GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }

            LoadLiveFeed();
        }

        protected void LoadLiveFeed()
        {
            try
            {
                var nodes = zooKeeper.GetChildren("/live_nodes/feed", new LiveFeedWatcher(this));
                FeedManager.Instance.ClearAndAddServer(nodes.ToArray());
            }
            catch { }
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.FEEDPROXY;
        }

        class LiveFeedWatcher : IWatcher
        {
            FeedProxyNode node;

            public LiveFeedWatcher(FeedProxyNode node)
            {
                this.node = node;
            }

            public void Process(WatchedEvent @event)
            {
                switch (@event.Type)
                {
                    case EventType.NodeChildrenChanged:
                        {
                            node.LoadLiveFeed();
                            Logger.GetLogger(node.BaseUrl).Info("detected feed node change");
                            break;
                        }
                }
            }
        }
    }
}