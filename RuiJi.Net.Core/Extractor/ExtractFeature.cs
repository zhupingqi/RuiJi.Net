using Newtonsoft.Json;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extractor.Selector;
using System.Collections.Generic;

namespace RuiJi.Net.Core.Extractor
{
    /// <summary>
    /// extract with feature
    /// </summary>
    public class ExtractFeature : IRuiJiParseResult
    {
        /// <summary>
        /// wildcard expression
        /// </summary>
        [JsonProperty("wildcard")]
        public string Wildcard { get; set; }

        /// <summary>
        /// match selectors
        /// </summary>
        [JsonProperty("features", ItemConverterType = typeof(ISelectorConverter))]
        public List<ISelector> Features { get; set; }
    }
}
