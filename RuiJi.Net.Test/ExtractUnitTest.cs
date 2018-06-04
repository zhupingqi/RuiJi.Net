using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Extracter.Enum;
using RuiJi.Net.Core.Extracter.Selector;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Owin;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class ExtractUnitTest
    {
        [TestMethod]
        public void TestLocalExtract()
        {
            var crawler = new RuiJiCrawler();
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

            //block.TileSelector.Metas.AddMeta(new ExtractBase {
            //    Name = "title",
            //    Selectors = new List<ISelector> {
            //        new CssSelector(".pt-cv-title")
            //    }
            //});

            //block.TileSelector.Metas.AddMeta(new ExtractBase
            //{
            //    Name = "url",
            //    Selectors = new List<ISelector> {
            //       new CssSelector(".pt-cv-readmore","href")
            //    }
            //});


            block.TileSelector.Metas.AddMeta("title",new List<ISelector> {
                new CssSelector(".pt-cv-title")
            });

            block.TileSelector.Metas.AddMeta("url", new List<ISelector> {
                new CssSelector(".pt-cv-readmore","href")
            });

            var r = RuiJiExtracter.Extract(content, block);

            Assert.IsTrue(r.Content.Length > 0);
            Assert.IsTrue(r.Tiles.Count > 0);
        }

        [TestMethod]
        public void TestNodeExtract()
        {
            ServerManager.StartServers();

            var response = new Crawler().Request("http://www.ruijihg.com/%e5%bc%80%e5%8f%91/");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return;

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

            block.TileSelector.Metas.AddMeta("title", new List<ISelector> {
                new CssSelector(".pt-cv-title")
            });

            block.TileSelector.Metas.AddMeta("url", new List<ISelector> {
                new CssSelector(".pt-cv-readmore","href")
            });

            var r = Extracter.Extract(new ExtractRequest {
                Block = block,
                Content = content
            });

            Assert.IsTrue(r.Content.Length > 0);
            Assert.IsTrue(r.Tiles.Count > 0);
        }

        [TestMethod]
        public void TestGetRule()
        {
            ServerManager.StartServers();

            var response = new Crawler().Request("http://www.ruijihg.com/2018/05/20/ruiji-solr-net/");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return;

            var content = response.Data.ToString();

            var block = Feeder.GetExtractBlock("http://www.ruijihg.com/2018/05/20/ruiji-solr-net/").First();

            var r = Extracter.Extract(new ExtractRequest
            {
                Block = block,
                Content = content
            });

            Assert.IsTrue(r.Content.Length > 0);
            Assert.IsTrue(r.Tiles.Count > 0);
        }
    }
}
