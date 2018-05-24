using LiteDB;
using RuiJi.Core.Extensions.System;
using RuiJi.Core.Utils.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Rule
{
    public class RuleLiteDB
    {
        public static List<RuleModel> GetRuleModels(Paging page)
        {
            using (var db = new LiteDatabase(@"Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");

                page.Count = col.Count();

                return col.Find(Query.All(), page.Start, page.PageSize).ToList();
            }
        }

        public static void AddOrUpdate(RuleModel rule)
        {
            using (var db = new LiteDatabase(@"Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");
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

        public static void Remove(int id)
        {
            using (var db = new LiteDatabase(@"Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");
                col.Delete(id);
            }
        }

        public static void CreateIndex()
        {
            using (var db = new LiteDatabase(@"Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("feeds");
                col.EnsureIndex(m => m.Domain);
            }
        }

        public static List<RuleModel> Match(string url)
        {
            url = url.Trim().ToLower();
            var domain = new Uri(url).GetDomain();

            using (var db = new LiteDatabase(@"Rules.db"))
            {
                var col = db.GetCollection<RuleModel>("rules");
                var rules = col.Find(Query.Where("Domain", m => m.AsString == domain)).ToList();

                return Match(rules, url);
            }
        }

        private static List<RuleModel> Match(List<RuleModel> rules,string url)
        {           
            rules = WildcardMatch(rules,url);
            if (rules == null)
                rules = ForwordMaxMatch(rules, url);
            return rules;
        }

        private static List<RuleModel> ForwordMaxMatch(List<RuleModel> rules, string url)
        {
            int maxLength = 1;
            RuleModel mRule = null;
            var uri = new Uri(url);

            foreach (var rule in rules)
            {
                int length = ForwordMatch(uri, new Uri(rule.Expression));
                if (maxLength < length)
                {
                    maxLength = length;
                    mRule = rule;
                }
            }

            return mRule == null ? null : rules.Where(r => r.Expression == mRule.Expression).OrderBy(r => r.Id).ToList();
        }

        private static int ForwordMatch(Uri extractUri, Uri uri)
        {
            int len = Math.Min(extractUri.AbsolutePath.Length, uri.AbsolutePath.Length);
            int i = 0;
            for (; i < len; i++)
            {
                if (extractUri.AbsolutePath[i] != uri.AbsolutePath[i])
                    break;
            }
            return i;
        }

        private static List<RuleModel> WildcardMatch(List<RuleModel> rules, string url)
        {
            int maxLength = 1;
            RuleModel mRule = null;
            var uri = new Uri(url);

            foreach (var rule in rules)
            {
                if (uri.WildcardMatch(rule.Expression))
                {
                    var length = rule.Expression.Length;
                    if (maxLength < length)
                    {
                        maxLength = length;
                        mRule = rule;
                    }
                }
            }

            if(mRule != null)
                return rules.Where(r => r.Expression == mRule.Expression).OrderBy(r => r.Id).ToList();

            return null;
        }
    }
}