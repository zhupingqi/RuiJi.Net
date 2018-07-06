using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    /// <summary>
    /// crawler elect request
    /// </summary>
    public class CrawlerElectRequest
    {
        /// <summary>
        /// uri need to visit
        /// </summary>
        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        /// <summary>
        /// if true,crawler proxy will elect ip
        /// </summary>
        [JsonProperty("electIp")]
        public bool ElectIp { get; set; }

        /// <summary>
        /// if true crawler proxy will elect proxy
        /// </summary>
        [JsonProperty("electProxy")]
        public bool ElectProxy { get; set; }
    }
}