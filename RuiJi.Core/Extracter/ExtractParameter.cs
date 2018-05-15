using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RuiJi.Core.Extracter
{
    public class ExtractParameter
    {
        [JsonProperty("collection")]
        public ExtractSelecterCollection SelectCollection { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }
    }
}
