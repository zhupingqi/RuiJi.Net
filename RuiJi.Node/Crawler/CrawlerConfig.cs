using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Crawler
{
    public class CrawlerConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("proxy")]
        public string Proxy { get; set; }

        [JsonProperty("baseUrl")]
        public string baseUrl { get; set; }

        [JsonProperty("ips")]
        public string[] Ips { get; set; }

        [JsonProperty("cookie")]
        public bool UseCookie { get; set; }
    }
}