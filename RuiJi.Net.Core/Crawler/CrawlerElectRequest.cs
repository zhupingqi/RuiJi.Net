using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    public class CrawlerElectRequest
    {
        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("electIp")]
        public bool ElectIp { get; set; }

        [JsonProperty("electProxy")]
        public bool ElectProxy { get; set; }
    }
}