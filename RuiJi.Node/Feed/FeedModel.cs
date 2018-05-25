using Newtonsoft.Json;
using RuiJi.Core.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Feed
{
    public enum FeedTypeEnum
    {
        HTML,
        XML,
        JSON,
        JSONP
    }

    public enum AddressExtractMethodEnum
    {
        AUTO,
        EXPRESSION,
        REGEX
    }

    public enum FeedStatus
    {
        ON,
        OFF
    }

    public class FeedModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sitename")]
        public string SiteName { get; set; }

        [JsonProperty("railling")]
        public string Railling { get; set; }

        /// <summary>
        /// url format by feedProxy
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("delay")]
        public int Delay { get; set; }

        [JsonProperty("type")]
        public FeedTypeEnum Type { get; set; }

        [JsonProperty("extractMethod")]
        public AddressExtractMethodEnum ExtractMethod { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("postParam")]
        public string PostParam { get; set; }

        [JsonProperty("ua")]
        public string UA { get; set; }

        [JsonProperty("headers")]
        public List<WebHeader> Headers { get; set; }

        [JsonProperty("rules")]
        public int Rules { get; set; }

        [JsonProperty("scheduling")]
        public string Scheduling { get; set; }

        [JsonProperty("status")]
        public FeedStatus Status { get; set; }
    }
}