using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extracter.Selector;

namespace RuiJi.Net.Core.Extracter
{
    public class ExtractFeatureBlock
    {
        [JsonProperty("feature")]
        public List<ISelector> Feature { get; set; }

        [JsonProperty("block")]
        public ExtractBlock Block { get; set; }

        public ExtractFeatureBlock()
        { }

        public ExtractFeatureBlock(ExtractBlock block, string feature)
        {
            this.Block = block;
            var selectors = new List<ISelector>();

            var sp = feature.Replace("\r\n", "\n").Split('\n');

            foreach (var s in sp)
            {
                var selector = RuiJiExtractBlockParser.ParserSelector(s);
                selectors.Add(selector);
            }

            this.Feature = selectors;
        }
    }

    public class ExtractRequest
    {
        public List<ExtractFeatureBlock> Blocks { get; set; }

        //[JsonProperty("content")]
        public string Content { get; set; }
    }
}
