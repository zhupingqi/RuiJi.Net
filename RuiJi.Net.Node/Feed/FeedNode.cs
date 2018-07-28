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
using RuiJi.Net.Core.Code.Compiler;
using RuiJi.Net.Core.Code.Provider;
using RuiJi.Net.Node.Compile;
using RuiJi.Net.Node.Feed.Db;

namespace RuiJi.Net.Node.Feed
{
    public class FeedNode : NodeBase
    {
        private FeedScheduler scheduler;

        public FeedNode(string baseUrl, string zkServer, string proxyUrl) : base(baseUrl, zkServer, proxyUrl)
        {

        }

        ~FeedNode()
        {
            FeedScheduler.GetSecheduler(BaseUrl).Stop();
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

            scheduler = new FeedScheduler();
            scheduler.Start(BaseUrl, this);
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.FEED;
        }

        public override void Stop()
        {
            if (scheduler != null)
            {
                scheduler.Stop();
                scheduler = null;
            }

            base.Stop();
        }
    }
}