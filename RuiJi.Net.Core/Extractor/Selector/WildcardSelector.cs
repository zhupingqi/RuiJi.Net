using Newtonsoft.Json;
using RuiJi.Net.Core.Extractor.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Selector
{
    public class WildcardSelector : SelectorBase
    {
        /// <summary>
        /// regex pattern
        /// </summary>
        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        public WildcardSelector(string pattern)
        {
            this.Pattern = pattern;
        }

        public WildcardSelector()
        {

        }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.WILDCARD;
        }

        public override string ToString()
        {
            var remove = Remove ? " -r" : "";

            return "wildcard " + Pattern + " " + remove;
        }
    }
}
