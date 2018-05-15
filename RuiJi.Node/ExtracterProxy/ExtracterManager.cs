using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.ExtracterProxy
{
    public class ExtracterManager
    {
        private static ExtracterManager _elector = null;
        private static object _lck = new object();

        private List<string> serverMap = new List<string>();

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

        public void AddServer(string baseUrl)
        {
            lock (_lck)
            {
                if (!serverMap.Contains(baseUrl))
                    serverMap.Add(baseUrl);
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
