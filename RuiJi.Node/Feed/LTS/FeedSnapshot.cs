using Newtonsoft.Json;
using RuiJi.Core.Extracter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Feed.LTS
{
    public class FeedSnapshot
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("block")]
        public ExtractBlock ExtractBlock { get; set; }
    }
}