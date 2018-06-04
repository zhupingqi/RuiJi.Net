using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node
{
    public class NodeConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("proxy")]
        public string Proxy { get; set; }

        [JsonProperty("baseUrl")]
        public string baseUrl { get; set; }

        [JsonProperty("pages")]
        public int[] Pages { get; set; }
    }
}