using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regards.Web.Seed;
using RuiJi.Core.Crawler;
using RuiJi.Core.Extracter;
using RuiJi.Core.Extracter.Enum;
using RuiJi.Core.Extracter.Selector;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class CrawlerProxyUnitTest
    {
        [TestMethod]
        public void TestSDKMethod()
        {
            RuiJi.Test.Common.StartupNodes();

            var request = new Request("http://www.ruijihg.com/%e5%bc%80%e5%8f%91/");

            var response = Crawler.Request(request);
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

            block.TileSelector.Metas.Add("title", new List<ISelector> {
                new CssSelector(".pt-cv-title")
            });

            block.TileSelector.Metas.Add("url", new List<ISelector> {
                new CssSelector(".pt-cv-readmore","href")
            });

            var extRequest = new ExtractRequest {
                Content = content,
                Block = block
            };

            var r = Extracter.Extract(extRequest);

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void TestProxyStatusScheduler()
        {
            ProxyStatusScheduler.Start();

            Thread.Sleep(300000);
            Assert.IsTrue(true);
        }
    }
}
