using LiteDB;
using RuiJi.Core.Utils.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Feed
{
    public class FeedLiteDb
    {
        public static List<FeedModel> GetFeedModels(Paging page)
        {
            using (var db = new LiteDatabase(@"Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                page.Count = col.Count(m => m.Rules >= 0);

                return col.Find(m=>m.Rules >= 0, page.Start, page.PageSize).ToList();                
            }
        }

        public static void CreateFeed(FeedModel feed)
        {
            using (var db = new LiteDatabase(@"Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                //feed = new FeedModel();
                ////feed.Id = Guid.NewGuid().ToString("N");
                //feed.SiteName = "新华网";
                //feed.Railling = "财经 滚动";
                //feed.Address = "http://www.news.cn/fortune/gd.htm";
                //feed.Domain = new Uri(feed.Address).Host;
                //feed.Type = FeedTypeEnum.HTML;
                //feed.Method = AddressExtractMethodEnum.AUTO;
                //feed.Status = FeedStatus.ON;

                //col.EnsureIndex(m => m.SiteName);
                //col.EnsureIndex(m => m.Rules);
                //col.EnsureIndex(m => m.Status);
                //col.EnsureIndex(m => m.Type);
                //col.EnsureIndex(m => m.Method);

                //col.Insert(feed);
            }
        }
    }
}