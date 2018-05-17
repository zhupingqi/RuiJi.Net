using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class CrawlerProxyUnitTest
    {
        [TestMethod]
        public void TestSDKMethod()
        {
            var request = new Request("http://www.baidu.com");

            var response = Crawler.Request(request);

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }
    }
}
