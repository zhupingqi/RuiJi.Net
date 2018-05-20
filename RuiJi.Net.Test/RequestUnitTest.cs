using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Core.Crawler;
using RuiJi.Owin;

namespace RuiJi.Net.Test
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

        [TestMethod]
        public void TestSessionCrawler()
        {
            ServerManager.StartServers();

            var crawler = new SessionCrawler();
            var request = new Request("http://www.baidu.com/");
            var response = crawler.Request(request);

            Assert.IsTrue(response.Headers.Count(m => m.Key == "Set-Cookie") > 0);

            request = new Request("http://www.baidu.com/about/");
            response = crawler.Request(request);

            Assert.IsTrue(response.Headers.Count(m => m.Key == "Set-Cookie") == 0);

            ServerManager.StopAll();
        }
    }
}
