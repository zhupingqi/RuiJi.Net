using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regards.Web.Seed;

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

        [TestMethod]
        public void TestProxyStatusScheduler()
        {
            ProxyStatusScheduler.Start();

            Thread.Sleep(300000);
            Assert.IsTrue(true);
        }
    }
}
