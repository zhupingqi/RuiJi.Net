using Newtonsoft.Json;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed.Db
{
    public enum FeedTypeEnum
    {
        HTML,
        XML,
        JS,
        JSON,
        JSONP
    }

    public class FeedModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sitename")]
        public string SiteName { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("delay")]
        public int Delay { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(EnumConvert<FeedTypeEnum>))]
        public FeedTypeEnum Type { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("postParam")]
        public string PostParam { get; set; }

        [JsonProperty("ua")]
        public string UA { get; set; }

        [JsonProperty("headers")]
        public List<WebHeader> Headers { get; set; }

        [JsonProperty("scheduling")]
        public string Scheduling { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(EnumConvert<Status>))]
        public Status Status { get; set; }

        [JsonProperty("feedonly")]
        public bool FeedOnly { get; set; }

        [JsonProperty("block")]
        public string BlockExpression { get; set; }

        [JsonProperty("rexp")]
        public string RuiJiExpression { get; set; }
    }
}