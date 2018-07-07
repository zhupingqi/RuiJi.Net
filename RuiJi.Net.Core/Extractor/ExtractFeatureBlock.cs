using Newtonsoft.Json;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor
{
    /// <summary>
    /// extract block with feature
    /// </summary>
    public class ExtractFeatureBlock
    {
        /// <summary>
        /// extract feature
        /// </summary>
        [JsonProperty]
        public ExtractFeature ExtractFeature { get; set; }

        /// <summary>
        /// extract block
        /// </summary>
        [JsonProperty("block")]
        public ExtractBlock Block { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public ExtractFeatureBlock()
        {
            ExtractFeature = new ExtractFeature();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="block">extract block</param>
        /// <param name="feature">extract feature</param>
        public ExtractFeatureBlock(ExtractBlock block, string feature = "")
        {
            this.Block = block;
            var selectors = new List<ISelector>();
            if (string.IsNullOrEmpty(feature))
                return;

            var sp = feature.Replace("\r\n", "\n").Split('\n');

            foreach (var s in sp)
            {
                var selector = RuiJiBlockParser.ParserSelector(s);
                selectors.Add(selector);
            }

            ExtractFeature = new ExtractFeature();

            ExtractFeature.Features = selectors;
        }
    }
}
