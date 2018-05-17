using Newtonsoft.Json;
using RestSharp;
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

            ConfigurationManager.AppSettings["cp"].Split(',').ToList().ForEach(m=> {
                proxys.Add(new RuiJiProxy {
                    Type = ProxyTypeEnum.Crawler,
                    BaseUrl = m,
                    Active = false
                });
            });

            ConfigurationManager.AppSettings["ep"].Split(',').ToList().ForEach(m => {
                proxys.Add(new RuiJiProxy
                {
                    Type = ProxyTypeEnum.Extracter,
                    BaseUrl = m,
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

        public void MarkDown(string proxyUrl)
        {
            var proxy = proxys.SingleOrDefault(m=>m.BaseUrl == proxyUrl);
            if (proxy != null)
                proxy.Active = false;
        }

        public string Elect(ProxyTypeEnum proxyType)
        {
            var p = proxys.Where(m => m.Type == proxyType && m.Active).OrderBy(m => m.Counts).FirstOrDefault();
            if(p != null)
            {
                p.Counts++;
                return p.BaseUrl;
            }

            return null;
        }

        public void RefreshStatus()
        {
            proxys.Where(m => m.Active == false).ToList().ForEach(m => {
                if (Ping(m))
                    m.Active = true;
            });
        }

        private bool Ping(RuiJiProxy proxy)
        {
            try
            {
                var resource = "api/cp/ping";
                if (proxy.Type == ProxyTypeEnum.Extracter)
                    resource = "api/ep/ping";

                var client = new RestClient("http://" + proxy.BaseUrl);
                var restRequest = new RestRequest(resource);
                restRequest.Method = Method.GET;
                restRequest.Timeout = 15000;

                var restResponse = client.Execute(restRequest);

                if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;

                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}