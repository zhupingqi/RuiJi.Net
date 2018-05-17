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

namespace RuiJi.Node.Extracter
{
    public class ExtracterNode : NodeBase
    {
        public ExtracterNode(string baseUrl, string zkServer, string proxyUrl) : base(baseUrl, zkServer, proxyUrl)
        {
            
        }

        public override void Start()
        {
            if (string.IsNullOrEmpty(ProxyUrl))
                throw new Exception("ProxyUrl must be set,call setup method!");

            base.Start();
        }

        protected override void OnStartup()
        {
            var stat = ZooKeeper.Exists("/live_nodes/extracter/" + BaseUrl, false);
            if (stat == null)
                ZooKeeper.Create("/live_nodes/extracter/" + BaseUrl, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);

            //create crawler config in zookeeper
            stat = ZooKeeper.Exists("/config/extracter/" + BaseUrl, false);
            if (stat == null)
            {
                var d = new ExtracterConfig()
                {
                    Name = BaseUrl,
                    baseUrl = BaseUrl,
                    Proxy = ProxyUrl
                };
                ZooKeeper.Create("/config/extracter/" + BaseUrl, JsonConvert.SerializeObject(d).GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }
        }
    }
}