using LiteDB;
using RuiJi.Net.Core.Utils.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace RuiJi.Net.Node.Feed.Db
{
    public class FeedLiteDb
    {
        /**
         * System.TypeInitializationException: The type initializer for 'RuiJi.Net.Node.Feed.Db.FeedLiteDb' threw an exception. --->
         * System.InvalidOperationException:
         * Your platform does not support FileStream.Lock. Please set mode=Exclusive in your connnection string to avoid this error. ---> System.PlatformNotSupportedException: Locking/unlocking file regions is not supported on this platform. Use FileShare on the entire file instead.
         *
         * fix:
         * OS X do not support file lock, so you can't use LiteDB with lock support (no multi process access). But you can still use LiteDB in multi thread model, using: "Mode=Exclusive" in connection string (exclusive mode do not lock file because open file in exclusive mode
         * https://github.com/mbdavid/LiteDB/issues/787
         * 
         */
        static readonly bool isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        /// 
        /// </summary>
        static readonly string connectionString = 
            isOSX ? @"Filename=LiteDb/Feeds.db;Mode=Exclusive"
                : @"LiteDb/Feeds.db";
        
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
            using (var db = new LiteDatabase(@"Filename=LiteDb/Feeds.db;Mode=Exclusive"))
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