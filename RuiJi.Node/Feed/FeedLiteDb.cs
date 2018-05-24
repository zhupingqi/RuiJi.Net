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

                page.Count = col.Count();

                return col.Find(m => true, page.Start, page.PageSize).ToList();
            }
        }

        public static void CreateFeed(FeedModel feed)
        {
            using (var db = new LiteDatabase(@"Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                col.Insert(feed);
            }
        }

        public static void CreateIndex()
        {
            using (var db = new LiteDatabase(@"Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");
                col.EnsureIndex(m => m.SiteName);
                col.EnsureIndex(m => m.Rules);
                col.EnsureIndex(m => m.Status);
                col.EnsureIndex(m => m.Type);
                col.EnsureIndex(m => m.Method);
            }
        }
    }
}