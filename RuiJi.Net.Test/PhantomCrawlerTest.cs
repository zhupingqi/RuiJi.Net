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
            request.Proxy = new RequestProxy();
            request.Proxy.Host = "http://180.118.86.178";
            request.Proxy.Port = 9000;
            var crawler = new PhantomCrawler();

            var response = crawler.Request(request);

            Assert.IsTrue(response.Data.ToString().Length > 0);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var request = new Request("https://gitee.com/zhupingqi/RuiJi.Net");
            request.Proxy = new RequestProxy();
            request.Proxy.Host = "http://180.118.86.178";
            request.Proxy.Port = 9000;
            request.RunJS = true;

            var crawler = new RuiJiCrawler();

            var response = crawler.Request(request);

            Assert.IsTrue(response.Data.ToString().Length > 0);
        }
    }
}
