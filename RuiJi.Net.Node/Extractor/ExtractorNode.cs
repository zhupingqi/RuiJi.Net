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

namespace RuiJi.Net.Node.Extractor
{
    public class ExtractorNode : NodeBase
    {
        public ExtractorNode(string baseUrl, string zkServer, string proxyUrl) : base(baseUrl, zkServer, proxyUrl)
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
            base.CreateLiveNode("/live_nodes/extractor/" + BaseUrl, null);

            //create extreacter config in zookeeper
            var stat = zooKeeper.Exists("/config/extractor/" + BaseUrl, false);
            if (stat == null)
            {
                var d = new NodeConfig()
                {
                    Name = BaseUrl,
                    baseUrl = BaseUrl,
                    Proxy = ProxyUrl,
                    Pages = new int[0]
                };
                zooKeeper.Create("/config/extractor/" + BaseUrl, JsonConvert.SerializeObject(d).GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.Extractor;
        }
    }
}