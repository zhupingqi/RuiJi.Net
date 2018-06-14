using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.Crawler;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class PhantomCrawlerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var request = new Request("http://www.baidu.com");
            var crawler = new PhantomCrawler();

            var response = crawler.Request(request);

            Assert.IsTrue(response.Data.ToString().Length > 0);
        }
    }
}
