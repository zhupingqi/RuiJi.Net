using Newtonsoft.Json;
using Org.Apache.Zookeeper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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