using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.RTS
{
    /// <summary>
    /// feed snapshot
    /// </summary>
    public class Snapshot
    {
        /// <summary>
        /// feed identity
        /// </summary>
        [JsonProperty("id")]
        public string FeedId { get; set; }

        /// <summary>
        /// request address
        /// </summary>
        [JsonProperty("requestUrl")]
        public string RequestUrl { get; set; }

        /// <summary>
        /// response address
        /// </summary>
        [JsonProperty("responseUrl")]
        public string ResponseUrl { get; set; }

        /// <summary>
        /// response content
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// ruiji expression
        /// </summary>
        [JsonProperty("expression")]
        public string Expression { get; set; }
    }
}