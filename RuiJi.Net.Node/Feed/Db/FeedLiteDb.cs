using LiteDB;
using RuiJi.Net.Core.Utils.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using RuiJi.Net.Storage;

namespace RuiJi.Net.Node.Feed.Db
{
    public class FeedLiteDb
    {
        private static readonly string connectionString = LiteDbStorageHelper.GetConnectionString(@"LiteDb/Feeds.db");
        
        static FeedLiteDb()
        {
            CreateIndex();
        }

        public static List<FeedModel> GetFeedModels(Paging page, string key = null, string method = null, string type = null, string status = null)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                Expression<Func<FeedModel, bool>> expression = x => true;
                if (!string.IsNullOrEmpty(key))
                    expression = expression.And(x => x.Address.Contains(key) || x.SiteName.Contains(key) || x.Remark.Contains(key));

                if (!string.IsNullOrEmpty(method) && method.ToLower() != "all" && method.ToLower() != "method")
                    expression = expression.And(x => x.Method == method);

                if (!string.IsNullOrEmpty(type) && type.ToLower() != "all" && type.ToLower() != "type")
                    expression = expression.And(x => x.Type.Equals(Enum.Parse(typeof(FeedTypeEnum), type.ToUpper())));

                if (!string.IsNullOrEmpty(status) && status.ToLower() != "all" && status.ToLower() != "status")
                    expression = expression.And(x => x.Status.Equals(Enum.Parse(typeof(Status), status.ToUpper())));

                page.Count = col.Count(expression);

                return col.Find(expression, page.Start, page.PageSize).ToList();
            }
        }

        public static List<FeedModel> GetAvailableFeeds(Paging page)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                page.Count = col.Count();

                return col.Find(m => m.Status == Status.ON).Skip(page.Start).Take(page.PageSize).ToList();
            }
        }

        public static List<FeedModel> GetFeedModels(int[] pages, int pageSize)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                var results = new List<FeedModel>();

                pages.ToList().ForEach((page) =>
                {
                    var start = (page - 1) * pageSize;

                    results.AddRange(col.Find(Query.All(), start, pageSize).Where(m => m.Status == Status.ON));
                });

                return results;
            }
        }

        public static void AddOrUpdate(FeedModel feed)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                if (feed.Id == 0)
                    col.Insert(feed);
                else
                    col.Update(feed);
            }
        }

        public static bool ChangeStatus(int[] ids, Status status)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FeedModel>("feeds");
                var list = col.Find(i => ids.Contains(i.Id)).ToList();
                list.ForEach((r) =>
                {
                    r.Status = status;
                });
                col.Update(list);
            }

            return true;
        }

        public static bool Remove(int id)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FeedModel>("feeds");
                return col.Delete(id);
            }
        }

        public static bool Remove(int[] ids)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FeedModel>("feeds");
                col.Delete(x => ids.Contains(x.Id));
            }

            return true;
        }

        public static void RemoveAll()
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FeedModel>("feeds");
                col.Delete(Query.All());
            }
        }

        public static void CreateIndex()
        {
            using (var db = new LiteDatabase(connectionString))
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
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FeedModel>("feeds");

                return col.Find(m => m.Id == id).FirstOrDefault();
            }
        }

        public static List<FeedModel> GetFeed(int[] ids)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FeedModel>("feeds");
                return col.Find(i => ids.Contains(i.Id)).ToList();
            }
        }
    }
}