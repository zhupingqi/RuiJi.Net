using RuiJi.Net.Core.Crawler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace RuiJi.Net.Test
{
    public class ProxyTest
    {
        [Fact]
        public void TestMethod1()
        {
            var proxys = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "proxy.txt")).Distinct().ToList();
            var results = new List<string>();

            foreach (var proxy in proxys)
            {
                var r = Ping(proxy);

                if(!string.IsNullOrEmpty(r))
                {
                    results.Add(r + "|" + proxy);
                }
            }

            File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "proxy_results.txt"),results.ToArray());
        }

        private string Ping(string addr)
        {
            try
            {
                var crawler = new RuiJiCrawler();
                var request = new Request("https://www.baidu.com/");
                request.Timeout = 5000;
                var sp = addr.Split(':');

                request.Proxy = new RequestProxy(sp[0], Convert.ToInt32(sp[1]));
                request.Proxy.Scheme = "https";

                var response = crawler.Request(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return "https";

                request.Proxy.Scheme = "http";

                response = crawler.Request(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return "http";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }
    }
}
