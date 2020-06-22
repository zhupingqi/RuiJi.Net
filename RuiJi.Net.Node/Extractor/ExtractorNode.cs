using Newtonsoft.Json;
using org.apache.zookeeper;
using System;
using System.Text;
using static org.apache.zookeeper.ZooDefs;

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
            var stat = zooKeeper.existsAsync("/config/extractor/" + BaseUrl, false).Result;
            if (stat == null)
            {
                var d = new NodeConfig()
                {
                    Name = BaseUrl,
                    baseUrl = BaseUrl,
                    Proxy = ProxyUrl,
                    Pages = new int[0]
                };
                zooKeeper.createAsync("/config/extractor/" + BaseUrl, Encoding.UTF8.GetBytes( JsonConvert.SerializeObject(d)), Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            }
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.EXTRACTOR;
        }
    }
}