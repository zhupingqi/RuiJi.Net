using Newtonsoft.Json;
using RuiJi.Core.Extracter;
using RuiJi.Core.Utils.Page;
using RuiJi.Node.Feed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RuiJi.Owin.Controllers
{
    public class FeedApiController : ApiController
    {
        [HttpGet]
        public object Feeds(int offset,int limit)
        {
            var auth = Request.RequestUri.Authority;

            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            return new
            {
                rows = FeedLiteDb.GetFeedModels(paging),
                total = paging.Count
            };
        }

        [HttpGet]
        public object Rules(int offset, int limit)
        {
            var auth = Request.RequestUri.Authority;

            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            return new
            {
                rows = RuleLiteDB.GetRuleModels(paging),
                total = paging.Count
            };
        }

        [HttpGet]
        public object UrlRule(string url)
        {
            return RuleLiteDB.Match(url).Select(m => JsonConvert.DeserializeObject<ExtractBlockCollection>( m.Blocks)).ToList();
        }

        [HttpGet]
        public object FeedJob(string feedUrl)
        {
            //according to feed url return available formated url
            //include random tick if need
            //need url format processor like below
            // {# datetime:yyyyMMdd #}
            // {# tick:10 #}

            return null;
        }
    }
}