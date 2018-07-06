using RuiJi.Net.Core.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core
{
    /// <summary>
    /// crawler server manager
    /// </summary>
    public class CrawlerServerManager
    {
        /// <summary>
        /// host visit model
        /// </summary>
        internal class HostVisit
        {
            public string IP { get; set; }

            public string Host { get; set; }

            public long LastVisitDate { get; set; }
        }

        /// <summary>
        /// crawler server
        /// </summary>
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

        /// <summary>
        /// crawler server list
        /// </summary>
        public List<Server> Servers { get; private set; }

        /// <summary>
        /// crawler server manager instance
        /// </summary>
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
            Servers = new List<Server>();
        }

        /// <summary>
        /// elect crawler
        /// </summary>
        /// <param name="uri">uri</param>
        /// <returns>crawler server elect result</returns>
        public CrawlerElectResult ElectIP(Uri uri)
        {
            lock (_lck)
            {
                if (Servers.Count == 0)
                    return null;

                if (!hostMap.ContainsKey(uri.Host))
                    hostMap.Add(uri.Host, 0);
                else
                    hostMap[uri.Host]++;

                var server = Servers[Convert.ToInt32(hostMap[uri.Host] % (ulong)Servers.Count)];

                return new CrawlerElectResult()
                {
                    BaseUrl = server.BaseUrl,
                    ClientIp = server.ClientIp
                };
            }
        }

        /// <summary>
        /// add a server to servers
        /// </summary>
        /// <param name="baseUrl">server base url</param>
        /// <param name="ips">server available ip addresses</param>
        public void AddServer(string baseUrl, string[] ips)
        {
            lock (_lck)
            {
                Servers.RemoveAll(m => m.BaseUrl == baseUrl);

                foreach (var ip in ips)
                {
                    var svr = new Server();
                    svr.BaseUrl = baseUrl;
                    svr.ClientIp = ip;

                    Servers.Add(svr);
                }
            }
        }

        /// <summary>
        /// remove server from servers
        /// </summary>
        /// <param name="baseUrl">server base url</param>
        public void RemoveServer(string baseUrl)
        {
            lock (_lck)
            {
                Servers.RemoveAll(m => m.BaseUrl == baseUrl);
            }
        }

        /// <summary>
        /// get server by server register ip
        /// </summary>
        /// <param name="ip">previously registered IP addresses</param>
        /// <returns>server</returns>
        public CrawlerElectResult GetServer(string ip)
        {
            lock (_lck)
            {
                var server = Servers.FirstOrDefault(m => m.ClientIp == ip);
                if (server == null)
                    return null;

                return new CrawlerElectResult
                {
                    BaseUrl = server.BaseUrl,
                    ClientIp = ip
                };
            }
        }

        /// <summary>
        /// clear servers
        /// </summary>
        public void Clear()
        {
            lock (_lck)
            {
                Servers.Clear();
            }
        }
    }
}
