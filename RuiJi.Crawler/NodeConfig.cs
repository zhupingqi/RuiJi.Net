using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Crawler
{
    public class NodeConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ip")]
        public string NodeIp { get; set; }

        [JsonProperty("crawlIps")]
        public string[] CrawlIps { get; set; }

        [JsonProperty("cache")]
        public TimeSpan Cache { get; set; }

        [JsonProperty("useCookie")]
        public bool UseCookie { get; set; }
    }
}