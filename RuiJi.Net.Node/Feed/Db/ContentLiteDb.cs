using LiteDB;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace RuiJi.Net.Node.Feed.Db
{
    public class ContentLiteDb
    {
        static ContentLiteDb()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LiteDb", "Content");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static List<ContentModel> GetModels(Paging page, string shard = "", int feedID = 0)
        {
            var connectString = LiteDbConfiguration.CONTENT;
            if (connectString.IndexOf("{shard}") != -1 && !string.IsNullOrEmpty(shard))
            {
                connectString = connectString.Replace("{shard}",shard);
            }

            using (var db = new LiteDatabase(connectString))
            {
                var col = db.GetCollection<ContentModel>("contents");

                var q = Query.All();

                if (feedID != 0)
                    q = Query.Where("FeedId", m => m.AsInt32 == feedID);

                page.Count = col.Count(q);

                return col.Find(q).OrderByDescending(m => m.Id).Skip(page.Start).Take(page.PageSize).ToList();
            }
        }

        public static bool Remove(int[] ids, string shard = "")
        {
            var connectString = LiteDbConfiguration.CONTENT;
            if (connectString.IndexOf("{shard}") != -1 && !string.IsNullOrEmpty(shard))
            {
                connectString = connectString.Replace("{shard}", shard);
            }

            using (var db = new LiteDatabase(connectString))
            {
                var col = db.GetCollection<ContentModel>("contents");

                col.Delete(x => ids.Contains(x.Id));
            }

            return true;
        }
    }
}