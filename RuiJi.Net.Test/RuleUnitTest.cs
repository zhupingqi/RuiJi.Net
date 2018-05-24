using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RuiJi.Core.Extensions.System;
using RuiJi.Core.Extracter;
using RuiJi.Core.Extracter.Enum;
using RuiJi.Core.Extracter.Selector;
using RuiJi.Node.Rule;

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

            var blocks = new ExtractBlockCollection
            {
                new ExtractBlock()
                {
                    Selectors = new List<ISelector>
                    {
                        new CssSelector("#main",CssTypeEnum.InnerHtml)
                    },
                    Metas = new ExtractMetaCollection()
                }
            };

            blocks[0].Metas.AddMeta("time", new List<ISelector> {
                new CssSelector("time",CssTypeEnum.Text)
            });

            blocks[0].Metas.AddMeta("author", new List<ISelector> {
                new CssSelector(".author",CssTypeEnum.Text)
            });

            blocks[0].Metas.AddMeta("content", new List<ISelector> {
                new CssSelector(".entry-content",CssTypeEnum.InnerHtml)
            });

            rule.Blocks = JsonConvert.SerializeObject(blocks);

            //rule.Id = 1;
            RuleLiteDB.AddOrUpdate(rule);

            Assert.IsTrue(rule.Id > 0);
        }

        [TestMethod]
        public void TestMatchRule()
        {
            var rules = RuleLiteDB.Match("http://www.ruijihg.com/2018/05/24/json-net-%e5%8f%8d%e5%ba%8f%e5%88%97%e5%8c%96%e6%8e%a5%e5%8f%a3%e9%9b%86%e5%90%88/");

            Assert.IsTrue(rules.Count > 0);
        }
    }
}
