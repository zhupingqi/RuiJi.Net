using RuiJi.Net.Core.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    public class CrawlerElectResult
    {
        public string BaseUrl { get; set; }

        public string ClientIp { get; set; }

        public RequestProxy Proxy { get; set; }
    }
}