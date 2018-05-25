using RuiJi.Core.Utils.Page;
using RuiJi.Node.Feed;
using RuiJi.Node.Rule;
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
    }
}