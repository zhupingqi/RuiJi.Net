using Newtonsoft.Json;

namespace RuiJi.Net.Node
{
    public class NodeData
    {
        [JsonProperty("stat")]
        public object Stat { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }
}