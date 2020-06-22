using Newtonsoft.Json;
using org.apache.zookeeper;
using System;
using System.Text;
using static org.apache.zookeeper.ZooDefs;

namespace RuiJi.Net.Node.Crawler
{
    public class CrawlerNode : NodeBase
    {
        public CrawlerNode(string baseUrl, string zkServer, string proxyUrl) : base(baseUrl, zkServer, proxyUrl)
        {
        }

        public override void Start()
        {
            if (string.IsNullOrEmpty(ProxyUrl))
                throw new Exception("ProxyUrl must be set,call setup method!");

            base.Start();
        }

        public CrawlerConfig GetNodeConfig()
        {
            try
            {
                if (zooKeeper != null && zooKeeper.getState() == ZooKeeper.States.CONNECTED)
                {
                    var b = zooKeeper.getDataAsync("/config/crawler/" + BaseUrl, false).Result.Data;
                    var r = System.Text.Encoding.UTF8.GetString(b);

                    return JsonConvert.DeserializeObject<CrawlerConfig>(r);
                }
            }
            catch
            {
            }

            return new CrawlerConfig();
        }

        protected override void OnStartup()
        {
            base.CreateLiveNode("/live_nodes/crawler/" + BaseUrl,null);

            //create crawler config in zookeeper
            var stat = zooKeeper.existsAsync("/config/crawler/" + BaseUrl, false).Result;
            if (stat == null)
            {
                var d = new CrawlerConfig()
                {
                    Name = BaseUrl,
                    baseUrl = BaseUrl,
                    Proxy = ProxyUrl,
                    Ips = new string[0],
                    UseCookie = true
                };
                zooKeeper.createAsync("/config/crawler/" + BaseUrl, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(d)), Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            }
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.CRAWLER;
        }
    }
}