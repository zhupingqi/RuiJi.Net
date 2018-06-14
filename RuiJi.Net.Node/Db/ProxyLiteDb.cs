using LiteDB;
using RuiJi.Net.Core.Utils.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Db
{
    public class ProxyLiteDb
    {
        private static int start = 0;
        private static object _lck = new object();

        static ProxyLiteDb()
        {
            CreateIndex();
        }

        public static List<ProxyModel> GetModels(Paging page)
        {
            using (var db = new LiteDatabase(@"LiteDb/Proxys.db"))
            {
                var col = db.GetCollection<ProxyModel>("proxys");

                page.Count = col.Count();

                return col.Find(Query.All(), page.Start, page.PageSize).ToList();
            }
        }

        public static List<ProxyModel> GetModels(int[] pages, int pageSize)
        {
            using (var db = new LiteDatabase(@"LiteDb/Proxys.db"))
            {
                var col = db.GetCollection<ProxyModel>("proxys");

                var results = new List<ProxyModel>();

                pages.ToList().ForEach((page) => {
                    var start = (page - 1) * pageSize;

                    results.AddRange(col.Find(Query.All(), start, pageSize));
                });

                return results;
            }
        }

        public static void AddOrUpdate(ProxyModel proxy)
        {
            using (var db = new LiteDatabase(@"LiteDb/Proxys.db"))
            {
                lock (_lck)
                {
                    var col = db.GetCollection<ProxyModel>("proxys");

                    if (proxy.Id == 0)
                    {
                        if (col.Count(m => m.Ip == proxy.Ip && m.Port == proxy.Port) == 0)
                            col.Insert(proxy);
                    }
                    else
                        col.Update(proxy);
                }
            }
        }

        public static void Remove(int id)
        {
            using (var db = new LiteDatabase(@"LiteDb/Proxys.db"))
            {
                lock (_lck)
                {
                    var col = db.GetCollection<ProxyModel>("proxys");
                    col.Delete(id);
                }
            }
        }

        public static bool Remove(int[] ids)
        {
            using (var db = new LiteDatabase(@"LiteDb/Proxys.db"))
            {
                lock (_lck)
                {
                    var col = db.GetCollection<ProxyModel>("proxys");
                    ids.ToList().ForEach((m) =>
                    {
                        col.Delete(m);
                    });
                }
            }

            return true;
        }

        public static void CreateIndex()
        {
            using (var db = new LiteDatabase(@"LiteDb/Proxys.db"))
            {
                var col = db.GetCollection<ProxyModel>("proxys");
                col.EnsureIndex(m => m.Ip);
                col.EnsureIndex(m => m.Port);
                col.EnsureIndex(m => m.Status);
            }
        }

        public static ProxyModel Get(int id)
        {
            using (var db = new LiteDatabase(@"LiteDb/Proxys.db"))
            {
                var col = db.GetCollection<ProxyModel>("proxys");

                return col.Find(m => m.Id == id).FirstOrDefault();
            }
        }

        public static ProxyModel Get()
        {
            using (var db = new LiteDatabase(@"LiteDb/Proxys.db"))
            {
                lock (_lck)
                {
                    var col = db.GetCollection<ProxyModel>("proxys");
                    var count = col.Count(m => m.Status == Status.ON);

                    if (count == 0)
                        return null;

                    if (start >= count)
                        start = 0;

                    return col.Find(m => m.Status == Status.ON).Skip(start++).Take(1).FirstOrDefault();
                }
            }
        }
    }
}