using LiteDB;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Node.Feed;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Db
{
    public class ContentLiteDb
    {
        static ContentLiteDb()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LiteDb", "Content");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static List<ContentModel> GetModels(Paging page, string shard, int feedID = 0)
        {
            using (var db = new LiteDatabase(@"LiteDb/Content/" + shard + ".db"))
            {
                var col = db.GetCollection<ContentModel>("contents");

                Expression<Func<ContentModel, bool>> expression = x => true;

                if (feedID != 0)
                    expression = expression.And(x => x.FeedId == feedID);

                page.Count = col.Count(expression);

                return col.Find(expression).OrderByDescending(m => m.Id).Skip(page.Start).Take(page.PageSize).ToList();
            }
        }

        public static bool Remove(int[] ids, string shard)
        {
            using (var db = new LiteDatabase(@"LiteDb/Content/" + shard + ".db"))
            {
                var col = db.GetCollection<ContentModel>("contents");

                col.Delete(x => ids.Contains(x.Id));
            }

            return true;
        }
    }
}