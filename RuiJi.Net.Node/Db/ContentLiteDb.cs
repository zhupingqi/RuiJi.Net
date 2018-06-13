using LiteDB;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Node.Feed;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Db
{
    public class ContentLiteDb
    {
        public static List<ContentModel> GetModels(Paging page, string shard, int feedID = 0)
        {
            using (var db = new LiteDatabase(@"LiteDb/Content/" + shard + ".db"))
            {
                var col = db.GetCollection<ContentModel>("contents");

                page.Count = col.Count();

                if (feedID == 0)
                    return col.Find(Query.All()).OrderByDescending(m => m.Id).Skip(page.Start).Take(page.PageSize).ToList();
                else
                    return col.Find(m=>m.Id == feedID).OrderByDescending(m=>m.Id).Skip(page.Start).Take(page.PageSize).ToList();
            }
        }
    }
}