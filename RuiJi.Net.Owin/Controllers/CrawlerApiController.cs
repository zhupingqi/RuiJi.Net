using Newtonsoft.Json;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Utils;
using RuiJi.Net;
using RuiJi.Net.Node.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using RuiJi.Net.NodeVisitor;

namespace RuiJi.Net.Owin.Controllers
{
    public class CrawlerApiController : ApiController
    {
        [HttpPost]
        [WebApiCacheAttribute(Duration = 10)]
        public Response Crawl(Request request)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLER)
            {
                var crawler = new RuiJiCrawler();
                var response = crawler.Request(request);

                var maxRefresh = 2;
                string refreshUrl;

                while (HasRefreshMeta(response, out refreshUrl) && maxRefresh > 0)
                {
                    crawler = new RuiJiCrawler();
                    request.Uri = new Uri(refreshUrl);
                    response = crawler.Request(request);

                    maxRefresh--;
                }

                return response;
            }
            else
            {
                return Crawler.Request(request);
            }
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
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLER)
            {
                return ((CrawlerNode)node).GetNodeConfig();
            }

            return new { };
        }

        [HttpGet]
        public string[] Ips()
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLER)
            {
                return IPHelper.GetHostIPAddress().Select(m => m.ToString()).ToArray();
            }

            return new string[0];
        }

        [HttpPost]
        public void SetIps([FromBody]string[] ips)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLER)
            {
                var path = "/config/crawler/" + Request.RequestUri.Authority;

                var data = node.GetData("/config/crawler/" + Request.RequestUri.Authority);
                var config = JsonConvert.DeserializeObject<CrawlerConfig>(data.Data);
                config.Ips = ips;
                node.SetData(path, JsonConvert.SerializeObject(config));
            }
        }
    }
}