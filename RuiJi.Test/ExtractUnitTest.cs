using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Core.Crawler;
using RuiJi.Core.Extracter;
using RuiJi.Core.Extracter.Enum;
using RuiJi.Core.Extracter.Selector;

namespace RuiJi.Test
{
    [TestClass]
    public class ExtractUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var crawler = new IPCrawler();
            var request = new Request("http://www.ruijihg.com/%e5%bc%80%e5%8f%91/");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var block = new ExtractBlock();
            block.Selectors = new List<ISelector>
            {
                new CssSelector(".entry-content",CssTypeEnum.InnerHtml)
            };

            block.TileSelector = new ExtractTile
            {
                Selectors = new List<ISelector>
                {
                    new CssSelector(".pt-cv-content-item",CssTypeEnum.InnerHtml)
                }
            };

            block.TileSelector.Metas.Add("title",new List<ISelector> {
                new CssSelector(".pt-cv-title")
            });

            block.TileSelector.Metas.Add("url", new List<ISelector> {
                new CssSelector(".pt-cv-readmore","href")
            });

            var ext = new RuiJiExtracter();
            var r = ext.Extract(content, block);

            Assert.IsTrue(r.Content.Length > 0);
            Assert.IsTrue(r.Tiles.Count > 0);

        }

        public void TestParserExtractExpression()
        {
            var exp = "{{ block }} {{ tile }} {{ end tile }} {{ end block }}";

            exp = "/b()";
        }
    }
}
