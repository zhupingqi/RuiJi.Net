using RuiJi.Node.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Extracter
{
    public class ExtracterManager
    {
        private static ExtracterManager _elector = null;
        private static object _lck = new object();

        private List<string> serverMap = new List<string>();

        private ulong count = 0;

        public static ExtracterManager Instance
        {
            get
            {
                return _elector;
            }
        }

        static ExtracterManager()
        {
            _elector = new ExtracterManager();
        }

        private ExtracterManager()
        {
        }

        public ElectResult Elect()
        {
            lock (_lck)
            {
                if (serverMap.Count == 0)
                    return null;

                var server = serverMap[Convert.ToInt32(count++ % (ulong)serverMap.Count)];

                return new ElectResult()
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
