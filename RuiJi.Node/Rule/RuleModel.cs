using Newtonsoft.Json;
using RuiJi.Core.Extracter;
using RuiJi.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Node.Rule
{
    public enum RuleTypeEnum
    {
        HTML,
        JSON,
        JSONP,
        XML
    }

    public class RuleModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("type")]
        public RuleTypeEnum Type { get; set; }

        [JsonProperty("expression")]
        public string Expression { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("feature")]
        public string[] Feature { get; set; }

        [JsonProperty("blocks")]
        public string Blocks { get; set; }
    }
}