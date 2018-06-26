using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor.Selector;

namespace RuiJi.Net.Core.Extractor
{
    public class ExtractFeature
    {
        [JsonProperty("wildcard")]
        public string Wildcard { get; set; }

        [JsonProperty("feature")]
        public List<ISelector> Feature { get; set; }
    }

    public class ExtractFeatureBlock
    {
        [JsonProperty]
        public ExtractFeature ExtractFeature { get; set; }

        [JsonProperty("block")]
        public ExtractBlock Block { get; set; }

        public ExtractFeatureBlock()
        {
            ExtractFeature = new ExtractFeature();
        }

        public ExtractFeatureBlock(ExtractBlock block, string feature = "")
        {
            this.Block = block;
            var selectors = new List<ISelector>();
            if (string.IsNullOrEmpty(feature))
                return;

            var sp = feature.Replace("\r\n", "\n").Split('\n');

            foreach (var s in sp)
            {
                var selector = RuiJiExtractBlockParser.ParserSelector(s);
                selectors.Add(selector);
            }

            ExtractFeature = new ExtractFeature();

            ExtractFeature.Feature = selectors;
        }
    }

    public class ExtractRequest
    {
        public List<ExtractFeatureBlock> Blocks { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
