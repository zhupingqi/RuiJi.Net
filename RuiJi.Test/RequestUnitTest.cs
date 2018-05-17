using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Core.Crawler;

namespace RuiJi.Test
{
    [TestClass]
    public class RequestUnitTest
    {
        [TestMethod]
        public void NoIpMethod()
        {
            //no ip
            var crawler = new IPCrawler();
            var request = new Request("http://www.baidu.com");
            var response = crawler.Request(request);

            Assert.AreEqual(response.ResponseUri.ToString() , "http://www.baidu.com");
        }

        [TestMethod]
        public void IpMethod()
        {
            //no ip
            var crawler = new IPCrawler();
            var request = new Request("http://www.baidu.com");
            request.Ip = "192.168.5.140";
            var response = crawler.Request(request);

            Assert.AreEqual(response.ResponseUri.ToString(), "http://www.baidu.com");
        }
    }
}
