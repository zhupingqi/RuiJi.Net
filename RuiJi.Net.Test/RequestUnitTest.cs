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

            Assert.AreEqual(response.ResponseUri.ToString(), "http://www.baidu.com");
        }

        [TestMethod]
        public void IpMethod()
        {
            //no ip
            var crawler = new RuiJiCrawler();
            var request = new Request("https://www.baidu.com");
            request.Ip = "192.168.31.196";
            var response = crawler.Request(request);

            Assert.AreEqual(response.ResponseUri.ToString(), "http://www.baidu.com");
        }

        [TestMethod]
        public void TestPost()
        {
            var url = "http://s.miaojian.net/api/client/stats/industry?type=0&top=5";

            var request = new Request(url);
            request.Method = "POST";
            request.Headers.Add(new WebHeader("Content-Type", "application/json"));
            //request.Cookie = "ASP.NET_SessionId=y4stpykzzg42fjqwhksho2a4; instanceId=f2f88812a95945508afe7e56e80726f0; captchaCode=CBPT; .ASPXAUTH=4D137F3E165271DA5DDF953A55B1518BDCFDDDAD0D41DF927B008859D9B0F58985D5728996734519B19EF10FB08C021A6F877F8C6B78CD6B430880133FFDFD3BFD4E26201714A6DE1C89C18E9361412C8CB9D7864745BDF95FE184E8A223AF1A43D7BC1166E45EFE27E6ACACCB64576B2A957CCB097C4FD4BF5FC2DDEA0643CEC6D88D5A3E2473366F900A92C3322058306CD797243988E54258DCE5C026EF14DF14E29078F99B9F885C00D6828375D9E99F41E8AB0C63388D471ED9B25EDBEC1655F332138ECBBA00F006AD6F0DABC3207A1758947FE55D32A5F208530E7F76DA38AD814B49B5FB4844E27230AB7A23544F92B480CBA2DF0112AF269B1B252F";
            request.Data = "{\"filter\":{\"dateRange\":{\"type\":\"month\",\"value\":[]},\"toneIds\":[25]},\"classifyId\":\"100\"}";

            var crawler = new RuiJiCrawler();
            var response = crawler.Request(request);

            Assert.IsTrue(response.Headers.Count > 0);
        }

        [TestMethod]
        public void TestJsonGet()
        {
            var url = "http://s.miaojian.net/api/client/classify?id=";

            var request = new Request(url);
            request.Headers.Add(new WebHeader("Content-Type", "application/json"));
            request.Cookie = "";

            var crawler = new RuiJiCrawler();
            var response = crawler.Request(request);

            Assert.IsTrue(response.Headers.Count > 0);
        }

        [TestMethod]
        public void TestJsonPost()
        {
            var url = "http://s.miaojian.net/api/client/stats/industry?type=0&top=5";

            var request = new Request(url);
            request.Method = "POST";
            request.Headers.Add(new WebHeader("Content-Type", "application/json"));
            request.Cookie = "";
            request.Data = "{\"filter\":{\"dateRange\":{\"type\":\"month\",\"value\":[]},\"toneIds\":[25]},\"classifyId\":\"100\"}";

            var crawler = new RuiJiCrawler();
            var response = crawler.Request(request);

            Assert.IsTrue(response.Headers.Count > 0);
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

        [TestMethod]
        public void TestRequestProxy()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("https://www.baidu.com");
            request.Proxy = new RequestProxy("223.93.172.248", 3128);

            var response = crawler.Request(request);

            Assert.AreEqual(response.ResponseUri.ToString(), "https://www.baidu.com");
        }

        [TestMethod]
        public void TestMime()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("http://img10.jiuxian.com/2018/0111/cd51bb851410404388155b3ec2c505cf4.jpg");
            var response = crawler.Request(request);

            var ex = response.Extensions;

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
