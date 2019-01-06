using RuiJi.Net.Core.Crawler;
using System.Linq;
using Xunit;

namespace RuiJi.Net.Test
{
    public class RequestUnitTest
    {
        [Fact]
        public void NoIpMethod()
        {
            //no ip
            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.baidu.com");
            var response = crawler.Request(request);

            Assert.Equal("https://www.baidu.com/",response.ResponseUri.ToString());
        }

        [Fact]
        public void IpMethod()
        {
            //no ip
            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.cannews.com.cn/2018/1121/185448.shtml");
            request.Ip = "192.168.31.32";
            var response = crawler.Request(request);

            Assert.Equal("https://www.baidu.com/", response.ResponseUri.ToString());
        }

        [Fact]
        public void TestPost()
        {
            var url = "http://s.miaojian.net/api/client/clipping";

            var request = new Request(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Data = "{\"page\":1,\"rows\":15,\"orderby\":\"newsDate\",\"sort\":\"desc\",\"meger\":true,\"filter\":{\"mediaTypeIds\":[1983],\"dateRange\":{\"type\":\"month\",\"value\":[]}},\"classifyId\":\"100\"}";

            var crawler = new RuiJiCrawler();
            var response = crawler.Request(request);

            Assert.True(response.Headers.Count > 0);
        }

        [Fact]
        public void TestJsonGet()
        {
            var url = "http://s.miaojian.net/api/client/classify?id=";

            var request = new Request(url);
            request.Headers.Add(new WebHeader("Content-Type", "application/json"));
            request.Cookie = "";

            var crawler = new RuiJiCrawler();
            var response = crawler.Request(request);

            Assert.True(response.Headers.Count > 0);
        }

        [Fact]
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

            Assert.True(response.Headers.Count > 0);
        }

        [Fact]
        public void TestSessionCrawler()
        {
            //ServerManager.StartServers();

            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.baidu.com/");
            var response = crawler.Request(request);

            Assert.True(response.Headers.Count(m => m.Name == "Set-Cookie") > 0);

            request = new Request("http://www.baidu.com/about/");
            response = crawler.Request(request);

            Assert.True(response.Headers.Count(m => m.Name == "Set-Cookie") == 0);

            request = new Request("http://www.kuaidaili.com/");
            response = crawler.Request(request);

            Assert.True(response.Headers.Count(m => m.Name == "Set-Cookie") == 0);
        }

        [Fact]
        public void TestRequestProxy()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("https://www.baidu.com");
            request.Proxy = new RequestProxy("223.93.172.248", 3128);

            var response = crawler.Request(request);

            Assert.Equal("https://www.baidu.com", response.ResponseUri.ToString());
        }

        [Fact]
        public void TestMime()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("http://img10.jiuxian.com/2018/0111/cd51bb851410404388155b3ec2c505cf4.jpg");
            var response = crawler.Request(request);

            var ex = response.Extensions;

            Assert.True(response.IsRaw);

            request = new Request("https://avatars0.githubusercontent.com/u/16769087?s=460&v=4");
            response = crawler.Request(request);

            Assert.True(response.IsRaw);

            request = new Request("http://www.baidu.com/");
            response = crawler.Request(request);

            Assert.False(response.IsRaw);
        }
    }
}
