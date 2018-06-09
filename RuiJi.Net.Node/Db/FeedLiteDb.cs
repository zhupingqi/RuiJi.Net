using LiteDB;
using RuiJi.Net.Core.Utils.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Db
{
    public class FeedLiteDb
    {
        static FeedLiteDb()
        {
            CreateIndex();
        }

        public static List<FeedModel> GetFeedModels(Paging page)
        {
            using (var db = new LiteDatabase(@"LiteDb/Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                page.Count = col.Count();

                return col.Find(Query.All(), page.Start, page.PageSize).ToList();
            }
        }

        public static List<FeedModel> GetFeedModels(int[] pages,int pageSize)
        {
            using (var db = new LiteDatabase(@"LiteDb/Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                var results = new List<FeedModel>();

                pages.ToList().ForEach((page)=> {
                    var start = (page - 1) * pageSize;

                    results.AddRange(col.Find(Query.All(), start, pageSize));
                });

                return results;
            }
        }

        public static void AddOrUpdate(FeedModel feed)
        {
            using (var db = new LiteDatabase(@"LiteDb/Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                if (feed.Id == 0)
                    col.Insert(feed);
                else
                    col.Update(feed);
            }
        }

        public static bool Remove(int id)
        {
            using (var db = new LiteDatabase(@"LiteDb/Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");
                return col.Delete(id);
            }
        }

        public static bool Remove(int[] ids)
        {
            using (var db = new LiteDatabase(@"LiteDb/Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");
                ids.ToList().ForEach((m) => {
                    col.Delete(m);
                });
            }

            return true;
        }

        public static void CreateIndex()
        {
            using (var db = new LiteDatabase(@"LiteDb/Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");
                col.EnsureIndex(m => m.SiteName);
                col.EnsureIndex(m => m.Status);
                col.EnsureIndex(m => m.Type);
                col.EnsureIndex(m => m.Id);
            }
        }

        public static FeedModel GetFeed(int id)
        {
            using (var db = new LiteDatabase(@"LiteDb/Feeds.db"))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                return col.Find(m=>m.Id == id).FirstOrDefault();
            }
        }
    }
}