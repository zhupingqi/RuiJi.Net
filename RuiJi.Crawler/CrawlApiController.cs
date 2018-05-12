using RuiJi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Crawler
{
    public class CrawlApiController : ApiController
    {
        [HttpPost]
        [WebApiCacheAttribute(Duration = 10)]
        public new Response Request(Request request)
        {
            var crawler = new IPCrawler(request.Ip);
            var response = crawler.Request(request);

            var maxRefresh = 5;
            string refreshUrl;

            while (HasRefreshMeta(response, out refreshUrl) && maxRefresh > 0)
            {
                crawler = new IPCrawler(request.Ip);
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
        public object T()
        {
            return new {
                msg = "success"
            };
        }
    }
}