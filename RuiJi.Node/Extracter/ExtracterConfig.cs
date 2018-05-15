using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Extracter
{
    public class ExtracterConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("proxy")]
        public string Proxy { get; set; }

        [JsonProperty("baseUrl")]
        public string baseUrl { get; set; }
    }
}