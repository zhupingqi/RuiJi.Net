using LiteDB;
using RuiJi.Net.Core.Utils.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Net.Storage;

namespace RuiJi.Net.Node.Feed.Db
{
    public class UALiteDb
    {
        static Random r = new Random();

        static UALiteDb()
        {
            CreateIndex();
        }

        private static readonly string COLLECTION = "uAs";
        
        private static readonly string connectionString = LiteDbStorageHelper.GetConnectionString(@"LiteDb/UAs.db");


        public static List<UAModel> GetModels(Paging page, int groupId)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<UAModel>(COLLECTION);
                Expression<Func<UAModel, bool>> expression = x => true;
                if (groupId != 0)
                    expression = expression.And(x => x.GroupId == groupId);

                page.Count = col.Count(expression);

                return col.Find(expression, page.Start, page.PageSize).ToList();
            }
        }

        public static void AddOrUpdate(UAModel ua)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<UAModel>(COLLECTION);

                if (ua.Id == 0)
                {
                    ua.Name = ua.Name.Trim();
                    ua.Value = ua.Value.Trim();

                    col.Insert(ua);
                }
                else
                {
                    col.Update(ua);
                }
            }
        }

        public static bool Remove(int[] ids)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<UAModel>(COLLECTION);

                col.Delete(x => ids.Contains(x.Id));
            }

            return true;
        }

        public static bool RemoveByGorup(int groupId)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<UAModel>(COLLECTION);

                col.Delete(x => groupId == x.GroupId);
            }

            return true;
        }

        public static UAModel Get(int id)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<UAModel>(COLLECTION);

                return col.FindOne(m => m.Id == id);
            }
        }

        public static string GetOne()
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<UAModel>("uAs");

                var count = col.Count();

                var n = r.Next(count);

                var m = col.FindAll().Skip(n).Take(1).First();

                return m.Value + "." + r.Next(m.Count);
            }
        }

        public static void CreateIndex()
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<UAModel>(COLLECTION);
                col.EnsureIndex(m => m.GroupId);
            }
        }
    }
}
