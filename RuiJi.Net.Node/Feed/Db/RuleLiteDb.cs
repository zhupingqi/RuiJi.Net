using LiteDB;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RuiJi.Net.Node.Feed.Db
{
    public class RuleLiteDb
    {
        static RuleLiteDb()
        {
            CreateIndex();
        }

        public static List<RuleModel> GetModels(Paging page, string key = null, string type = null, string status = null)
        {
            using (var db = new LiteDatabase(@"LiteDb/Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");

                Expression<Func<RuleModel, bool>> expression = x => true;
                if (!string.IsNullOrEmpty(key))
                    expression = expression.And(x => x.Domain.Contains(key) || x.Expression.Contains(key) || x.Url.Contains(key));


                if (!string.IsNullOrEmpty(type) && type.ToLower() != "all" && type.ToLower() != "type")
                    expression = expression.And(x => x.Type.Equals(Enum.Parse(typeof(RuleTypeEnum), type.ToUpper())));


                if (!string.IsNullOrEmpty(status) && status.ToLower() != "all" && status.ToLower() != "status")
                    expression = expression.And(x => x.Status.Equals(Enum.Parse(typeof(Status), status.ToUpper())));

                page.Count = col.Count(expression);

                return col.Find(expression, page.Start, page.PageSize).ToList();
            }
        }

        public static void AddOrUpdate(RuleModel rule)
        {
            using (var db = new LiteDatabase(@"LiteDb/Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");
                if (Uri.IsWellFormedUriString(rule.Url, UriKind.Absolute))
                    rule.Domain = new Uri(rule.Url).GetDomain();

                if (rule.Id == 0)
                {
                    rule.Url = rule.Url.Trim().ToLower();
                    rule.Expression = rule.Expression.Trim().ToLower();

                    col.Insert(rule);
                }
                else
                {
                    col.Update(rule);
                }
            }
        }

        public static bool Remove(int id)
        {
            using (var db = new LiteDatabase(@"LiteDb/Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");
                return col.Delete(id);
            }
        }

        public static bool Remove(int[] ids)
        {
            using (var db = new LiteDatabase(@"LiteDb/Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");

                col.Delete(c => ids.Contains(c.Id));
            }

            return true;
        }

        public static bool StatusChange(int[] ids, Status status)
        {

            using (var db = new LiteDatabase(@"LiteDb/Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");
                var list = col.Find(i => ids.Contains(i.Id)).ToList();
                list.ForEach((r) =>
                {
                    r.Status = status;
                });
                col.Update(list);
            }

            return true;
        }

        public static void CreateIndex()
        {
            using (var db = new LiteDatabase(@"LiteDb/Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("feeds");
                col.EnsureIndex(m => m.Domain);
            }
        }

        public static List<RuleModel> Match(string url)
        {
            url = url.Trim().ToLower();
            var domain = new Uri(url).GetDomain();

            using (var db = new LiteDatabase(@"LiteDb/Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");
                var rules = col.Find(Query.Where("Domain", m => m.AsString == domain)).ToList();
                var masks = rules.Select(m => m.Expression).ToArray();

                var mask = Wildcard.MaxMatch(url, masks);
                var results = rules.Where(m => m.Expression == mask).ToList();

                return results;
            }
        }

        public static RuleModel Get(int id)
        {
            using (var db = new LiteDatabase(@"LiteDb/Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");

                return col.Find(m => m.Id == id).FirstOrDefault();
            }
        }
    }
}