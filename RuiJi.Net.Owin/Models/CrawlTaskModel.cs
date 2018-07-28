using Newtonsoft.Json;

namespace RuiJi.Net.Owin.Models
{
    public class CrawlTaskModel
    {
        [JsonProperty("feedId")]
        public int FeedId { get; set; }

        [JsonProperty("taskId")]
        public int TaskId { get; set; }

        [JsonProperty("content")]
        public bool IncludeContent { get; set; }
    }
}