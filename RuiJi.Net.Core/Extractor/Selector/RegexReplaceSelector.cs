using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Net.Core.Extractor.Enum;

namespace RuiJi.Net.Core.Extractor.Selector
{
    /// <summary>
    /// regex replace selector
    /// </summary>
    public class RegexReplaceSelector : SelectorBase
    {
        /// <summary>
        /// regex pattern
        /// </summary>
        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        /// <summary>
        /// replacement string
        /// </summary>
        [JsonProperty("newString")]
        public string NewString { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public RegexReplaceSelector()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pattern">regex replace pattern</param>
        /// <param name="NewString"></param>
        public RegexReplaceSelector(string pattern, string NewString)
        {
            this.NewString = NewString;
            this.Pattern = pattern;
        }

        /// <summary>
        /// set selector type enum
        /// </summary>
        /// <returns>selector type</returns>
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.REGEXREPLACE;
        }

        public override string ToString()
        {
            var remove = Remove ? " -r" : "";

            if (string.IsNullOrEmpty(NewString))
            {
                return "regR /" + Pattern + "/" + remove;
            }
            else
            {
                return "regR /" + Pattern + "/ " + NewString + remove;
            }
        }
    }
}
