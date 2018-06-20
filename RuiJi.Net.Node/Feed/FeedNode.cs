using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;
using System.Threading;
using RuiJi.Net.Core;
using Newtonsoft.Json;
using RuiJi.Net.Node.Feed.LTS;

namespace RuiJi.Net.Node.Feed
{
    public class FeedNode : NodeBase
    {
        public FeedNode(string baseUrl, string zkServer, string proxyUrl) : base(baseUrl, zkServer, proxyUrl)
        {

        }

        ~FeedNode()
        {
            FeedScheduler.Stop();
            FeedExtractScheduler.Stop();
        }

        protected override void OnStartup()
        {
            base.CreateLiveNode("/live_nodes/feed/" + BaseUrl, null);

            var stat = zooKeeper.Exists("/config/feed/" + BaseUrl, false);
            if (stat == null)
            {
                var d = new NodeConfig()
                {
                    Name = BaseUrl,
                    baseUrl = BaseUrl,
                    Proxy = ProxyUrl
                };
                zooKeeper.Create("/config/feed/" + BaseUrl, JsonConvert.SerializeObject(d).GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }

            FeedScheduler.Start(BaseUrl, ProxyUrl, this);
            FeedExtractScheduler.Start(BaseUrl);
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.FEED;
        }
    }
}