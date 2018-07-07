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
    /// <summary>
    /// extract request
    /// </summary>
    public class ExtractRequest
    {
        /// <summary>
        /// extract feature collection
        /// </summary>
        public List<ExtractFeatureBlock> Blocks { get; set; }

        /// <summary>
        /// The content that needs to be extracted
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
