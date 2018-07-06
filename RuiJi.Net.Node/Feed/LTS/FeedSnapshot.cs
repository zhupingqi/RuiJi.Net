using Newtonsoft.Json;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Node.Feed.Db;

namespace RuiJi.Net.Node.Feed.LTS
{
    public class FeedSnapshot
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("useBlock")]
        public bool UseBlock { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(EnumConvert<FeedTypeEnum>))]
        public FeedTypeEnum Type { get; set; }

        [JsonProperty("block")]
        public string BlockExpression { get; set; }

        [JsonProperty("rexp")]
        public string RuiJiExpression { get; set; }
    }
}