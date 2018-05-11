using RuiJi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace RuiJi.Crawler.Proxy
{
    public class ProxyServer
    {
        private static ProxyServer _instance;
        private ZooKeeper zookeeper;
        private LiveNodeWatch watcher;

        static ProxyServer()
        {
            _instance = new ProxyServer();
        }

        private ProxyServer()
        {

        }

        public static ProxyServer Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Start(string connectString)
        {
            zookeeper = new ZooKeeper(connectString, TimeSpan.MaxValue, watcher);
            watcher = new LiveNodeWatch();

            var stat = zookeeper.Exists("/live_nodes" , false);
            if (stat == null)
                zookeeper.Create("/live_nodes", Encoding.UTF8.GetBytes(""), null, CreateMode.Persistent);

            LoadLiveNodes();
        }

        public void Stop()
        {
            zookeeper.Dispose();
            zookeeper = null;
        }

        private void InitListener()
        {
            var cache = new PathChildrenCache(client, "/live_nodes", true);
            cache.start();
            cache.getListenable().addListener(new LiveNodesListener());
        }

        public void UpdateLiveNodes(string ip)
        {
            var ip = d.getPath().Replace("/live_nodes/", "");
            var content = Encoding.Default.GetString(d.getData());
            var cips = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (state == 0)
                ServerElector.Instance.AddCrawlerServer(ip, cips);
            else if (state == 1)
                ServerElector.Instance.RemoveCrawlerServer(ip);
            else
            {
                ServerElector.Instance.RemoveCrawlerServer(ip);
                ServerElector.Instance.AddCrawlerServer(ip, cips);
            }
        }

        public void LoadLiveNodes()
        {
            CrawlProxy.Instance.Clear();

            var nodes = zookeeper.GetChildren("/live_nodes", false);

            foreach (var node in nodes)
            {
                var ips = GetNodeIp(node);
                CrawlProxy.Instance.AddCrawlerServer(node,ips.ToList());
            }
        }

        private string[] GetNodeIp(string serverIp)
        {
            var b = zookeeper.GetData("/config/" + serverIp + "/ip.txt", false, null);
            var r = System.Text.Encoding.UTF8.GetString(b);

            return r.Split('\n');
        }

        public void DeleteTimeoutComplete(string path)
        {
            var state = client.checkExists().forPath(path);
            if (state != null)
                client.delete().forPath(path);
        }

        private void Log(string serverIp, string clientIp, string url)
        {
            try
            {
                var logFile = AppDomain.CurrentDomain.BaseDirectory + "log/" + DateTime.Now.ToString("yyyyMMddHH") + ".txt";
                var log = DateTime.Now.ToString() + "\t" + serverIp + "\t" + clientIp + "\t" + url;
                File.AppendAllLines(logFile, new string[] { log });
            }
            catch { }
        }

        private void LogTime(string url, string action, string time)
        {
            try
            {
                var logFile = AppDomain.CurrentDomain.BaseDirectory + "log_time/" + DateTime.Now.ToString("yyyyMMddHH") + ".txt";
                var log = DateTime.Now.ToString() + "\t" + url + "\t" + action + "\t" + time;
                File.AppendAllLines(logFile, new string[] { log });
            }
            catch { }
        }
    }
}