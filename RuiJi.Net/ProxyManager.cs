using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net
{
    public class ProxyManager
    {
        private static ProxyManager _instance;

        private List<RuiJiProxy> proxys;

        static ProxyManager()
        {
            _instance = new ProxyManager();
        }

        private ProxyManager()
        {
            proxys = new List<RuiJiProxy>();

            ConfigurationManager.AppSettings["crawler"].Split(',').ToList().ForEach(m=> {
                proxys.Add(new RuiJiProxy {
                    Type = ProxyTypeEnum.Crawler,
                    baseUrl = m,
                    Active = false
                });
            });

            ConfigurationManager.AppSettings["extracter"].Split(',').ToList().ForEach(m => {
                proxys.Add(new RuiJiProxy
                {
                    Type = ProxyTypeEnum.Extracter,
                    baseUrl = m,
                    Active = false
                });
            });

            RefreshStatus();
        }

        public static ProxyManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public void MarkDown(string baseUrl)
        {
            
        }

        public string Elect(ProxyTypeEnum proxyType)
        {
            throw new NotImplementedException();
        }

        public void RefreshStatus()
        {
            throw new NotImplementedException();
        }
    }
}