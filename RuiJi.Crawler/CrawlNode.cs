using RuiJi.Core.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Crawler
{
    public class CrawlNode
    {
        private static CrawlNode _instance;
        private ZooKeeper zookeeper;

        static CrawlNode()
        {
            _instance = new CrawlNode();
        }

        private CrawlNode()
        {
            
        }

        public static CrawlNode Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Start()
        {
            var zkServer = ConfigurationManager.AppSettings.Get("zkServer");
            var baseUrl = ConfigurationManager.AppSettings.Get("baseUrl");

            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                throw new ConfigurationErrorsException("baseUrl is not well formed!");
            }

            var u = new Uri(baseUrl);
            var clientIp = u.Host.ToLower();
            var clientPort = u.Port;

            if (!IPHelper.IsHostIPAddress(IPAddress.Parse(clientIp)))
            {
                throw new ConfigurationErrorsException("baseUrl is not allowed to use localhost or 127.0.0.1!");
            }

            zookeeper = new ZooKeeper(zkServer, TimeSpan.FromSeconds(300), null);

            var stat = zookeeper.Exists("/live_nodes", false);
            if(stat == null)
                zookeeper.Create("/live_nodes", null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);

            stat = zookeeper.Exists("/live_nodes/" + clientIp, false);
            if (stat == null)
                zookeeper.Create("/live_nodes/" + clientIp, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);
            
            //zookeeper.SetData("/live_nodes/" + clientIp, Encoding.UTF8.GetBytes(""), -1);
        }

        public void Stop()
        {
            zookeeper.Dispose();
            zookeeper = null;
        }
    }
}