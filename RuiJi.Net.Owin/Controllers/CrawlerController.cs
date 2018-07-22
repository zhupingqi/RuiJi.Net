using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Crawler;
using RuiJi.Net.NodeVisitor;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RuiJi.Net.Owin.Controllers
{
    [Produces("application/json")]
    [Route("api/crawler")]
    public class CrawlerController : ControllerBase
    {
        [HttpPost]
        [Route("request")]
        public Response Crawl([FromBody]Request request)
        {
            var node = ServerManager.Get(Request.Host.Value);

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
                        refreshUrl = new Uri(response.Request.Uri, refreshUrl).ToString();
                    }

                    return true;
                }
            }
            refreshUrl = "";
            return false;
        }

        [HttpGet]
        [Route("info")]
        public object ServerInfo()
        {
            var node = ServerManager.Get(Request.Host.Value);

            if (node.NodeType == Node.NodeTypeEnum.CRAWLER)
            {
                return ((CrawlerNode)node).GetNodeConfig();
            }

            return new { };
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.CRAWLER, RouteArgumentName = "baseUrl")]
        [Route("ips")]
        public string[] Ips(string baseUrl)
        {
            return IPHelper.GetHostIPAddress().Select(m => m.ToString()).ToArray();
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.CRAWLER, RouteArgumentName = "baseUrl")]
        [Route("ips")]
        public void SetIps([FromBody]string[] ips,string baseUrl)
        {
            var node = ServerManager.Get(Request.Host.Value);

            var path = "/config/crawler/" + Request.Host.Value;

            var data = node.GetData("/config/crawler/" + Request.Host.Value);
            var config = JsonConvert.DeserializeObject<CrawlerConfig>(data.Data);
            config.Ips = ips;
            node.SetData(path, JsonConvert.SerializeObject(config));
        }
    }
}