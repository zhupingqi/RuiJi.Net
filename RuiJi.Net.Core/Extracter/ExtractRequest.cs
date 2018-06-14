using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RuiJi.Net.Core.Extracter
{
    public class ExtractFeatureBlock
    {
        [JsonProperty("feature")]
        public string Feature { get; set; }

        [JsonProperty("block")]
        public ExtractBlock Block { get; set; }

        [JsonProperty("runJs")]
        public bool RunJS { get; set; }

        public ExtractFeatureBlock()
        { }

        public ExtractFeatureBlock(ExtractBlock block,string feature)
        {
            this.Block = block;
            this.Feature = feature;
        }
    }

    public class ExtractRequest
    {
        public List<ExtractFeatureBlock> Blocks { get; set; }

        //[JsonProperty("content")]
        public string Content { get; set; }
    }
}
