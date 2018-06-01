using Newtonsoft.Json;
using RuiJi.Core;
using RuiJi.Core.Utils;
using RuiJi.Node.Extracter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;
using LiteDB;

namespace RuiJi.Node.Feed
{
    public class FeedProxyNode : NodeBase
    {
        public FeedProxyNode(string baseUrl, string zkServer) : base(baseUrl, zkServer)
        {
            
        }

        protected override void OnStartup()
        {
            var stat = zooKeeper.Exists("/live_nodes/proxy/" + BaseUrl, false);
            if (stat == null)
                zooKeeper.Create("/live_nodes/proxy/" + BaseUrl, "feed proxy".GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);

            //create crawler proxy config in zookeeper
            stat = zooKeeper.Exists("/config/proxy/" + BaseUrl, false);
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
                            Console.WriteLine("detected feed node change");
                            break;
                        }
                }
            }
        }
    }
}