using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RuiJi.Net.Core.Extensions.System;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Enum;
using RuiJi.Net.Core.Extractor.Selector;
using RuiJi.Net.Node.Feed;
using RuiJi.Net.Node.Db;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class RuleUnitTest
    {
        [TestMethod]
        public void TestCreateRule()
        {
            //RuleLiteDB.Remove(1);

            var rule = new RuleModel();
            rule.Url = "http://www.ruijihg.com/2018/05/24/json-net-%e5%8f%8d%e5%ba%8f%e5%88%97%e5%8c%96%e6%8e%a5%e5%8f%a3%e9%9b%86%e5%90%88/";
            rule.Domain = new Uri(rule.Url).GetDomain();
            rule.Expression = "http://www.ruijihg.com/????/??/??/*";

            var block = new ExtractBlock()
            {
                Selectors = new List<ISelector>
                    {
                        new CssSelector("#main",CssTypeEnum.InnerHtml)
                    },
                Metas = new ExtractMetaCollection()
            };

            block.Metas.AddMeta("time", new List<ISelector> {
                new CssSelector("time",CssTypeEnum.Text)
            });

            block.Metas.AddMeta("author", new List<ISelector> {
                new CssSelector(".author",CssTypeEnum.Text)
            });

            block.Metas.AddMeta("content", new List<ISelector> {
                new CssSelector(".entry-content",CssTypeEnum.InnerHtml)
            });

            rule.BlockExpression = JsonConvert.SerializeObject(block);

            //rule.Id = 1;
            RuleLiteDb.AddOrUpdate(rule);

            Assert.IsTrue(rule.Id > 0);
        }

        [TestMethod]
        public void TestMatchRule()
        {
            TestLock(20);

            var rules = RuleLiteDb.Match("http://www.ruijihg.com/2018/05/24/json-net-%e5%8f%8d%e5%ba%8f%e5%88%97%e5%8c%96%e6%8e%a5%e5%8f%a3%e9%9b%86%e5%90%88/");

            Assert.IsTrue(rules.Count > 0);
        }

        [TestMethod]
        public void TestLock(int i)
        {
            lock (this)
            {
                if (i > 10)
                {
                    i--;
                    TestLock(i);
                }
            }
        }

        [TestMethod]
        public void TestRuleConvert()
        {
            var rule = new RuleModel();
            rule.Type = RuleTypeEnum.JSONP;
            var j = JsonConvert.SerializeObject(rule);

            var o = JsonConvert.DeserializeObject<RuleModel>(j);

            Assert.IsTrue(o.Type == RuleTypeEnum.JSONP);
        }
    }
}
