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
                if (zooKeeper != null && zooKeeper.State == ZooKeeper.States.CONNECTED)
                {
                    var b = zooKeeper.GetData("/config/crawler/" + BaseUrl, false, null);
                    var r = System.Text.Encoding.UTF8.GetString(b);

                    return JsonConvert.DeserializeObject<CrawlerConfig>(r);
                }
            }
            catch { }

            return new CrawlerConfig();
        }

        protected override void OnStartup()
        {
            base.CreateLiveNode("/live_nodes/crawler/" + BaseUrl,null);

            //create crawler config in zookeeper
            var stat = zooKeeper.Exists("/config/crawler/" + BaseUrl, false);
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
                zooKeeper.Create("/config/crawler/" + BaseUrl, JsonConvert.SerializeObject(d).GetBytes(), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }
        }

        protected override NodeTypeEnum SetNodeType()
        {
            return NodeTypeEnum.CRAWLER;
        }
    }
}