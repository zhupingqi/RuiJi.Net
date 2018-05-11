using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Crawler
{
    public class CrawlServer
    {
        private static CrawlServer _instance;
        private ZooKeeper zookeeper;

        static CrawlServer()
        {
            _instance = new CrawlServer();
        }

        private CrawlServer()
        {
            
        }

        public static CrawlServer Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Start(NodeSetting setting)
        {
            var connectString = setting.ZkServer.Ip + ":" + setting.ZkServer.Port;

            zookeeper = new ZooKeeper(connectString, TimeSpan.MaxValue, null);

            var stat = zookeeper.Exists("/live_nodes/" + setting.Client.Ip, false);
            if (stat == null)
                zookeeper.Create("/live_nodes/" + setting.Client.Ip, Encoding.UTF8.GetBytes(""), null, CreateMode.Ephemeral);
            else
                zookeeper.SetData("/live_nodes/" + setting.Client.Ip, Encoding.UTF8.GetBytes(""), -1);
        }

        public void Stop()
        {
            zookeeper.Dispose();
            zookeeper = null;
        }
    }
}