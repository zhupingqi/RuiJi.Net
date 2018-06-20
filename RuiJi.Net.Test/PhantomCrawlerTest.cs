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
            //request.Proxy = new RequestProxy();
            //request.Proxy.Host = "http://180.118.86.178";
            //request.Proxy.Port = 9000;
            var crawler = new RuiJiCrawler();

            var response = crawler.Request(request);

            Assert.IsTrue(response.Data.ToString().Length > 0);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var request = new Request("https://gitee.com/zhupingqi/RuiJi.Net");
            //request.Proxy = new RequestProxy();
            //request.Proxy.Host = "https://223.93.172.248";
            //request.Proxy.Port = 3128;
            //request.RunJS = true;

            var crawler = new RuiJiCrawler();

            var response = crawler.Request(request);

            Assert.IsTrue(response.Data.ToString().Length > 0);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var request = new Request("http://www.ruijihg.com/");
            request.Proxy = new RequestProxy();
            request.Proxy.Ip = "180.118.86.178";
            request.Proxy.Port = 9000;
            request.Proxy.Scheme = "http";
            request.RunJS = true;

            var crawler = new RuiJiCrawler();

            var response = crawler.Request(request);

            Assert.IsTrue(response.Data.ToString().Length > 0);
        }

        [TestMethod]
        public void TestMethod4()
        {
            var request = new Request("https://gitee.com/zhupingqi/RuiJi.Net");
            request.RunJS = true;
            //request.Cookie = "oschina_new_user=false;expires=Wed, 16 Jun 2038 06:57:20 GMT; domain=gitee.com; path=/,aliyungf_tc=AQAAAMt2pVc2cQkACw8UZUJNd5CbXTu0;expires=Wed, 16 Jun 2038 06:57:20 GMT; domain=gitee.com; path=/,oschina_new_user=false;expires=Wed, 16 Jun 2038 06:57:20 GMT; domain=gitee.com; path=/,user_locale=zh-CN;expires=Wed, 16 Jun 2038 06:57:20 GMT; domain=gitee.com; path=/,gitee-session-n=BAh7CEkiD3Nlc3Npb25faWQGOgZFVEkiJTVmYzc3OTQ4ZTRhNGM1MWM5MzI2YjQyOTI1MjRhOGMzBjsAVEkiF21vYnlsZXR0ZV9vdmVycmlkZQY7AEY6CG5pbEkiEF9jc3JmX3Rva2VuBjsARkkiMThCakFMNzlvVXhnNExxcmIwZWxWVFJzS2JMbFRWTHlzcGlJdVpqZWJiaHc9BjsARg%3D%3D--aff6f894a55d2ce1a7be4b3fa036bb95b2b0c68a;expires=Wed, 16 Jun 2038 06:57:20 GMT; domain=.gitee.com; path=/";

            var crawler = new RuiJiCrawler();

            var response = crawler.Request(request);

            Assert.IsTrue(response.Data.ToString().Length > 0);
        }
    }
}
