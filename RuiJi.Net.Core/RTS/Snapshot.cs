using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.RTS
{
    public class Snapshot
    {
        [JsonProperty("id")]
        public string FeedId { get; set; }

        [JsonProperty("requestUrl")]
        public string RequestUrl { get; set; }

        [JsonProperty("responseUrl")]
        public string ResponseUrl { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("expression")]
        public string Expression { get; set; }
    }
}