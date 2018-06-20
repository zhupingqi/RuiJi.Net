using RuiJi.Net.Core.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core
{
    public class CrawlerServerManager
    {
        internal class HostVisit
        {
            public string IP { get; set; }
            public string Host { get; set; }
            public long LastVisitDate { get; set; }
        }

        public class Server
        {
            public string BaseUrl { get; set; }
            public string ClientIp { get; set; }
        }

        private static CrawlerServerManager _elector = null;
        private static object _lck = new object();

        private List<HostVisit> visits = new List<HostVisit>();
        private Dictionary<string, List<string>> ipMap = new Dictionary<string, List<string>>();
        private Dictionary<string, ulong> hostMap = new Dictionary<string, ulong>();

        public List<Server> ServerMap { get; private set; }

        public static CrawlerServerManager Instance
        {
            get
            {
                return _elector;
            }
        }

        static CrawlerServerManager()
        {
            _elector = new CrawlerServerManager();
        }

        private CrawlerServerManager()
        {
            ServerMap = new List<Server>();
        }

        public CrawlerElectResult ElectIP(Uri uri)
        {
            lock (_lck)
            {
                if (ServerMap.Count == 0)
                    return null;

                if (!hostMap.ContainsKey(uri.Host))
                    hostMap.Add(uri.Host, 0);
                else
                    hostMap[uri.Host]++;

                var server = ServerMap[Convert.ToInt32(hostMap[uri.Host] % (ulong)ServerMap.Count)];

                return new CrawlerElectResult()
                {
                    BaseUrl = server.BaseUrl,
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

        public void AddServer(string baseUrl, string[] ips)
        {
            lock (_lck)
            {
                ServerMap.RemoveAll(m => m.BaseUrl == baseUrl);

                foreach (var ip in ips)
                {
                    var svr = new Server();
                    svr.BaseUrl = baseUrl;
                    svr.ClientIp = ip;

                    ServerMap.Add(svr);
                }
            }
        }

        public void RemoveServer(string baseUrl)
        {
            lock (_lck)
            {
                ServerMap.RemoveAll(m => m.BaseUrl == baseUrl);
            }
        }

        public CrawlerElectResult GetServer(string ip)
        {
            var server = ServerMap.FirstOrDefault(m => m.ClientIp == ip);
            if (server == null)
                return null;

            return new CrawlerElectResult
            {
                BaseUrl = server.BaseUrl,
                ClientIp = ip
            };
        }

        public void Clear()
        {
            lock (_lck)
            {
                ServerMap.Clear();
            }
        }
    }
}
