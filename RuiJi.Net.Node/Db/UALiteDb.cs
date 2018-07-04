using LiteDB;
using RuiJi.Net.Core.Utils.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Db
{
    public class UALiteDb
    {
        static UALiteDb()
        {
            CreateIndex();
        }

        public static List<UAModel> GetModels(Paging page, int groupId)
        {
            using (var db = new LiteDatabase(@"LiteDb/UAs.db"))
            {
                var col = db.GetCollection<UAModel>("uAs");
                Expression<Func<UAModel, bool>> expression = x => true;
                if (groupId != 0)
                    expression = expression.And(x => x.GroupId == groupId);

                page.Count = col.Count(expression);

                return col.Find(expression, page.Start, page.PageSize).ToList();
            }
        }

        public static void AddOrUpdate(UAModel ua)
        {
            using (var db = new LiteDatabase(@"LiteDb/UAs.db"))
            {
                var col = db.GetCollection<UAModel>("uAs");

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
            using (var db = new LiteDatabase(@"LiteDb/UAs.db"))
            {
                var col = db.GetCollection<UAModel>("uAs");

                col.Delete(x => ids.Contains(x.Id));
            }

            return true;
        }

        public static bool RemoveByGorup(int groupId)
        {
            using (var db = new LiteDatabase(@"LiteDb/UAs.db"))
            {
                var col = db.GetCollection<UAModel>("uAs");

                col.Delete(x => groupId == x.GroupId);
            }

            return true;
        }

        public static UAModel Get(int id)
        {
            using (var db = new LiteDatabase(@"LiteDb/UAs.db"))
            {
                var col = db.GetCollection<UAModel>("uAs");

                return col.FindOne(m => m.Id == id);
            }
        }

        public static void CreateIndex()
        {
            using (var db = new LiteDatabase(@"LiteDb/UAs.db"))
            {
                var col = db.GetCollection<UAModel>("uAs");
                col.EnsureIndex(m => m.GroupId);
            }
        }
    }
}
