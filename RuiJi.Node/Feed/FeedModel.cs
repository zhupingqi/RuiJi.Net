using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("type")]
        public FeedTypeEnum Type { get; set; }

        [JsonProperty("method")]
        public AddressExtractMethodEnum Method { get; set; }

        [JsonProperty("rules")]
        public int Rules { get; set; }

        [JsonProperty("scheduling")]
        public string Scheduling { get; set; }

        [JsonProperty("status")]
        public FeedStatus Status { get; set; }
    }
}