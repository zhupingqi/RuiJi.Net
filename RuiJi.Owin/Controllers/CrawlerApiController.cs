using RuiJi.Core;
using RuiJi.Core.Crawler;
using RuiJi.Core.Utils;
using RuiJi.Node.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Owin.Controllers
{
    public class CrawlerApiController : ApiController
    {
        [HttpPost]
        //[WebApiCacheAttribute(Duration = 10)]
        public new Response Request(Request request)
        {
            var crawler = new IPCrawler();
            var response = crawler.Request(request);

            var maxRefresh = 5;
            string refreshUrl;

            while (HasRefreshMeta(response, out refreshUrl) && maxRefresh > 0)
            {
                crawler = new IPCrawler();
                request.Uri = new Uri(refreshUrl);
                response = crawler.Request(request);

                maxRefresh--;
            }

            return response;
        }

        private bool HasRefreshMeta(Response response, out string refreshUrl)
        {
            if (!response.IsRaw)
            {
                var reg = new Regex("<meta[\\s]+http-equiv=\"Refresh\"[\\s]+content=['\"]?[\\d]+;URL=([^'\"]*)['\"]?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var ms = reg.Matches(response.Data.ToString());
                if (ms.Count > 0)
                {
                    refreshUrl = ms[0].Groups[1].Value;
                    if (!Uri.IsWellFormedUriString(refreshUrl, UriKind.Absolute))
                    {
                        refreshUrl = new Uri(response.RequestUri, refreshUrl).ToString();
                    }

                    return true;
                }
            }
            refreshUrl = "";
            return false;
        }

        [HttpGet]
        public object ServerInfo()
        {
            //var inst = CrawlerNodeService.Instance;

            //return new
            //{
            //    name = "RuiJi_Crawler_" + inst.BaseUrl,
            //    baseUrl = inst.BaseUrl,
            //    zkServer = inst.ZkServer,
            //    zkState = CrawlerNodeService.Instance.States,
            //    ips = IPHelper.GetHostIPAddress().Select(m => m.ToString()).ToArray(),
            //    cips = CrawlerNodeService.Instance.GetNodeConfig()
            //};

            return new { };
        }
    }
}