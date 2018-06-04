using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Crawler
{
    public class CrawlerConfig : NodeConfig
    {
        [JsonProperty("ips")]
        public string[] Ips { get; set; }

        [JsonProperty("cookie")]
        public bool UseCookie { get; set; }
    }
}