using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Proxy
{
    public class CrawlProxy
    {
        internal class HostVisit
        {
            public string IP { get; set; }
            public string Host { get; set; }
            public long LastVisitDate { get; set; }
        }

        internal class Server
        {
            public string ServerIp { get; set; }
            public string ClientIp { get; set; }
        }

        private static CrawlProxy _elector = null;
        private static object _lck = new object();

        private List<HostVisit> visits = new List<HostVisit>();
        private Dictionary<string, List<string>> ipMap = new Dictionary<string, List<string>>();
        private Dictionary<string, ulong> hostMap = new Dictionary<string, ulong>();
        private List<Server> serverMap = new List<Server>();

        public static CrawlProxy Instance
        {
            get
            {
                return _elector;
            }
        }

        static CrawlProxy()
        {
            _elector = new CrawlProxy();
        }

        private CrawlProxy()
        {
        }

        public ElectResult ElectIP(Uri uri)
        {
            lock (_lck)
            {
                if (!hostMap.ContainsKey(uri.Host))
                    hostMap.Add(uri.Host, 0);
                else
                    hostMap[uri.Host]++;

                var server = serverMap[Convert.ToInt32(hostMap[uri.Host] % (ulong)serverMap.Count)];

                return new ElectResult()
                {
                    ServerIp = server.ServerIp,
                    ClientIp = server.ClientIp
                };
            }
        }

        public void Schedule(string ip, string host)
        {
            lock (_lck)
            {
                if (hostMap.ContainsKey(host))
                    hostMap.Add(host, 1);
                else
                    hostMap[host] = hostMap[host]++;
            }
        }

        public void AddCrawlerServer(string serverIp, List<string> ips)
        {
            RemoveCrawlerServer(serverIp);

            lock (_lck)
            {
                foreach (var ip in ips)
                {
                    var svr = new Server();
                    svr.ServerIp = serverIp;
                    svr.ClientIp = ip;

                    serverMap.Add(svr);
                }
            }
        }

        public void RemoveCrawlerServer(string serverIp)
        {
            lock (_lck)
            {
                serverMap.RemoveAll(m => m.ServerIp == serverIp);
            }
        }

        public void Clear()
        {
            lock (_lck)
            {
                serverMap.Clear();
            }
        }
    }
}
