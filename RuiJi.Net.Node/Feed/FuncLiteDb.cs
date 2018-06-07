using LiteDB;
using RuiJi.Net.Core.Utils.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed
{
    public class FuncLiteDb
    {
        static FuncLiteDb()
        {
            CreateIndex();
        }

        public static List<FuncModel> GetFuncModels(Paging page)
        {
            using (var db = new LiteDatabase(@"LiteDb/Funcs.db"))
            {
                var col = db.GetCollection<FuncModel>("funcs");

                page.Count = col.Count();

                return col.Find(Query.All(), page.Start, page.PageSize).ToList();
            }
        }

        public static void AddOrUpdate(FuncModel rule)
        {
            using (var db = new LiteDatabase(@"LiteDb/Funcs.db"))
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
            using (var db = new LiteDatabase(@"LiteDb/Funcs.db"))
            {
                var col = db.GetCollection<FuncModel>("funcs");
                col.Delete(id);
            }
        }

        public static void CreateIndex()
        {
            using (var db = new LiteDatabase(@"LiteDb/Funcs.db"))
            {
                var col = db.GetCollection<FuncModel>("funcs");
                col.EnsureIndex(m => m.Name);
            }
        }

        public static FuncModel GetFunc(int id)
        {
            using (var db = new LiteDatabase(@"LiteDb/Funcs.db"))
            {
                var col = db.GetCollection<FuncModel>("funcs");

                return col.Find(m => m.Id == id).FirstOrDefault();
            }
        }

        public static FuncModel GetFunc(string name)
        {
            using (var db = new LiteDatabase(@"LiteDb/Funcs.db"))
            {
                var col = db.GetCollection<FuncModel>("funcs");

                return col.Find(m => m.Name == name).FirstOrDefault();
            }
        }
    }
}
