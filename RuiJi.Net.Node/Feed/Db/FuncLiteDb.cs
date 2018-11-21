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
    public class FuncLiteDb
    {
        private static readonly string connectionString = LiteDbStorageHelper.GetConnectionString(@"LiteDb/Funcs.db");

        static FuncLiteDb()
        {
            CreateIndex();
        }

        public static List<FuncModel> GetModels(Paging page, string type = "")
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FuncModel>("funcs");
                Expression<Func<FuncModel, bool>> expression = x => true;

                if (!string.IsNullOrEmpty(type))
                    expression = expression.And(x => ((FuncType)Convert.ToInt32(x.Type)).ToString().ToLower() == type.ToLower());

                page.Count = col.Count(expression);

                if (string.IsNullOrEmpty(type))
                    return col.Find(expression, page.Start, page.PageSize).ToList();
                else
                    return col.Find(expression, page.Start, page.PageSize).ToList();
            }
        }

        public static void AddOrUpdate(FuncModel rule)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FuncModel>("funcs");

                if (rule.Id == 0)
                {
                    rule.Name = rule.Name.Trim();
                    rule.Code = rule.Code.Trim();
                    rule.Sample = rule.Sample.Trim();

                    col.Insert(rule);
                }
                else
                {
                    col.Update(rule);
                }
            }
        }

        public static void Remove(int id)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FuncModel>("funcs");
                col.Delete(id);
            }
        }

        public static bool Remove(int[] ids)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FuncModel>("funcs");

                col.Delete(x => ids.Contains(x.Id));
            }

            return true;
        }

        public static void CreateIndex()
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FuncModel>("funcs");
                col.EnsureIndex(m => m.Name);
            }
        }

        public static FuncModel Get(int id)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FuncModel>("funcs");

                return col.Find(m => m.Id == id).FirstOrDefault();
            }
        }

        public static FuncModel Get(string name, FuncType funcType = FuncType.URLFUNCTION)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<FuncModel>("funcs");

                return col.Find(m => m.Name == name && m.Type == funcType).FirstOrDefault();
            }
        }
    }
}
