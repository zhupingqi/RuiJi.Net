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

namespace RuiJi.Node.Crawler
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
            if (zooKeeper != null && zooKeeper.State == ZooKeeper.States.CONNECTED)
            {
                var b = zooKeeper.GetData("/config/crawler/" + BaseUrl, false, null);
                var r = System.Text.Encoding.UTF8.GetString(b);

                return JsonConvert.DeserializeObject<CrawlerConfig>(r);
            }

            return new CrawlerConfig();
        }

        protected override void OnStartup()
        {
            var stat = zooKeeper.Exists("/live_nodes/crawler/" + BaseUrl, false);
            if (stat == null)
                zooKeeper.Create("/live_nodes/crawler/" + BaseUrl, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);

            //create crawler config in zookeeper
            stat = zooKeeper.Exists("/config/crawler/" + BaseUrl, false);
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
    }
}