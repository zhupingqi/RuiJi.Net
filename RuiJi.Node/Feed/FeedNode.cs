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

namespace RuiJi.Node.Feed
{
    public class FeedNode : NodeBase
    {
        public FeedNode(string baseUrl, string zkServer, string proxyUrl) : base(baseUrl, zkServer, proxyUrl)
        {

        }

        protected override void OnStartup()
        {
            var stat = zooKeeper.Exists("/live_nodes/feed/" + BaseUrl, false);
            if (stat == null)
                zooKeeper.Create("/live_nodes/feed/" + BaseUrl, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);

            stat = zooKeeper.Exists("/config/feed/" + BaseUrl, false);
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
        }
    }
}