using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Owin;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class RequestUnitTest
    {
        [TestMethod]
        public void NoIpMethod()
        {
            //no ip
            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.baidu.com");
            var response = crawler.Request(request);

            Assert.AreEqual(response.ResponseUri.ToString() , "http://www.baidu.com");
        }

        [TestMethod]
        public void IpMethod()
        {
            //no ip
            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.baidu.com");
            request.Ip = "192.168.5.140";
            var response = crawler.Request(request);

            Assert.AreEqual(response.ResponseUri.ToString(), "http://www.baidu.com");
        }

        [TestMethod]
        public void TestSessionCrawler()
        {
            //ServerManager.StartServers();

            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.baidu.com/");
            var response = crawler.Request(request);

            Assert.IsTrue(response.Headers.Count(m => m.Name == "Set-Cookie") > 0);

            request = new Request("http://www.baidu.com/about/");
            response = crawler.Request(request);

            Assert.IsTrue(response.Headers.Count(m => m.Name == "Set-Cookie") == 0);

            request = new Request("http://www.kuaidaili.com/");
            response = crawler.Request(request);

            Assert.IsTrue(response.Headers.Count(m => m.Name == "Set-Cookie") == 0);
        }
        

        public void TestPhantomjs()
        {

        }

        [TestMethod]
        public void TestRequestProxy()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.baidu.com");
            request.Proxy = new RequestProxy("115.223.233.34",9000);

            var response = crawler.Request(request);

            Assert.AreEqual(response.ResponseUri.ToString(), "http://www.baidu.com");
        }

        [TestMethod]
        public void TestMime()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("http://img10.jiuxian.com/2018/0111/cd51bb851410404388155b3ec2c505cf4.jpg");
            var response = crawler.Request(request);

            Assert.IsTrue(response.IsRaw);

            request = new Request("https://avatars0.githubusercontent.com/u/16769087?s=460&v=4");
            response = crawler.Request(request);

            Assert.IsTrue(response.IsRaw);

            request = new Request("http://www.baidu.com/");
            response = crawler.Request(request);

            Assert.IsFalse(response.IsRaw);
        }
    }
}
