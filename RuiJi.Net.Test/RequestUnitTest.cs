using System;
using System.Linq;
using System.Text;
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
        public void TestPost2()
        {
            var url = "http://www.qzggzy.com/FrontWeb/ggshow.aspx?Type=zfcg&BigType=10&findtxt=";

            var request = new Request(url);
            request.Method = "POST";
            request.Cookie = "ASP.NET_SessionId=4dsvttfwmcriljen221jaabg";
            var s = System.Web.HttpUtility.UrlEncode("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");

            var v = System.Web.HttpUtility.UrlEncode("zOmeaeSdclb5sF7Bh3dF2xYtRF7gNRE18XHHmxRF6s9WyDO7V3zKTsZP67G51gLrXnPzp/fA8RjyxNawmvHc4X1OiAj7ghNU3LAJlsF+0/4+onuVdpPAKKmJZdDtog0CAwS40SphESndm/dwXYu5GncCtSCoX6qyHyo9k8gDJ6L8ArPJ7X5mJX8aicMMimCBZkGu0TYTtqFI+vvDUOuoKVYTCPGRmzt3dyio9QfPC9reAkkAgGIou/PNvSXaH/3WiFR+wN7YVQrj3DdhuXW+wQqvc85+6sDzOLgsOMPeeZcVkqWkSHNzuRN9DJPQ6EamtchsPibFBj+wzYfd28R4Iw==");

            request.Data = string.Format("__EVENTTARGET=ctl15&__EVENTARGUMENT=&__VIEWSTATE={0}&__VIEWSTATEENCRYPTED=&__EVENTVALIDATION={1}&zp__PRONAME=&gvList%24ctl02%24hid_id=41447&gvList%24ctl02%24hidinid=0&gvList%24ctl03%24hid_id=41421&gvList%24ctl03%24hidinid=0&gvList%24ctl04%24hid_id=41409&gvList%24ctl04%24hidinid=0&gvList%24ctl05%24hid_id=41395&gvList%24ctl05%24hidinid=0&gvList%24ctl06%24hid_id=41382&gvList%24ctl06%24hidinid=0&gvList%24ctl07%24hid_id=41343&gvList%24ctl07%24hidinid=0&gvList%24ctl08%24hid_id=41346&gvList%24ctl08%24hidinid=0&gvList%24ctl09%24hid_id=41332&gvList%24ctl09%24hidinid=0&gvList%24ctl10%24hid_id=41345&gvList%24ctl10%24hidinid=0&gvList%24ctl11%24hid_id=41350&gvList%24ctl11%24hidinid=0&gvList%24ctl12%24hid_id=41313&gvList%24ctl12%24hidinid=0&gvList%24ctl13%24hid_id=41314&gvList%24ctl13%24hidinid=0&gvList%24ctl14%24hid_id=41293&gvList%24ctl14%24hidinid=0&gvList%24ctl15%24hid_id=41257&gvList%24ctl15%24hidinid=0&gvList%24ctl16%24hid_id=41256&gvList%24ctl16%24hidinid=0&ctl17=", s, v);

            var crawler = new RuiJiCrawler();
            var response = crawler.Request(request);

            Assert.IsTrue(response.Headers.Count > 0);
        }

        [TestMethod]
        public void TestPost()
        {
            var url = "http://s.miaojian.net/api/client/clipping";

            var request = new Request(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Data = "{\"page\":1,\"rows\":15,\"orderby\":\"newsDate\",\"sort\":\"desc\",\"meger\":true,\"filter\":{\"mediaTypeIds\":[1983],\"dateRange\":{\"type\":\"month\",\"value\":[]}},\"classifyId\":\"100\"}";

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
