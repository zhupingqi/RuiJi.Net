using Newtonsoft.Json;
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
        public Response Crawl(Request request)
        {
            var crawler = new IPCrawler();
            var response = crawler.Request(request);

            var maxRefresh = 2;
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
            var node = ServerManager.GetNode(Request.RequestUri.Port.ToString()) as CrawlerNode;

            return node.GetNodeConfig();
        }

        [HttpGet]
        public string[] Ips()
        {
            return IPHelper.GetHostIPAddress().Select(m=>m.ToString()).ToArray();
        }

        [HttpPost]
        public void SetIps([FromBody]string[] ips)
        {
            var node = ServerManager.GetNode(Request.RequestUri.Port.ToString()) as CrawlerNode;
            var path = "/config/crawler/" + Request.RequestUri.Authority;

            var data = node.GetData("/config/crawler/" + Request.RequestUri.Authority);
            var config = JsonConvert.DeserializeObject<CrawlerConfig>(data.Data);
            config.Ips = ips;
            node.SetData(path,JsonConvert.SerializeObject(config));
        }
    }
}