using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Node.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Extractor
{
    public class ExtractorManager
    {
        private static ExtractorManager _elector = null;
        private static object _lck = new object();

        private List<string> serverMap = new List<string>();

        private ulong count = 0;

        public static ExtractorManager Instance
        {
            get
            {
                return _elector;
            }
        }

        static ExtractorManager()
        {
            _elector = new ExtractorManager();
        }

        private ExtractorManager()
        {
        }

        public CrawlerElectResult Elect()
        {
            lock (_lck)
            {
                if (serverMap.Count == 0)
                    return null;

                var server = serverMap[Convert.ToInt32(count++ % (ulong)serverMap.Count)];

                return new CrawlerElectResult()
                {
                    BaseUrl = server
                };
            }
        }

        public void AddServer(string baseUrl)
        {
            lock (_lck)
            {
                if (!serverMap.Contains(baseUrl))
                    serverMap.Add(baseUrl);
            }
        }

        public void ClearAndAddServer(string[] baseUrls)
        {
            lock (_lck)
            {
                serverMap.Clear();
                foreach (var baseUrl in baseUrls)
                {
                    serverMap.Add(baseUrl);
                }
            }
        }

        public void RemoveServer(string baseUrl)
        {
            lock (_lck)
            {
                serverMap.Remove(baseUrl);
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
