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
    public class RegexSplitSelector : SelectorBase
    {
        /// <summary>
        /// regex pattern
        /// </summary>
        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        /// <summary>
        /// results index after split
        /// </summary>
        [JsonProperty("index")]
        public int[] Index { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pattern">pattern</param>
        /// <param name="index">result index</param>
        /// <param name="remove">remove flag</param>
        public RegexSplitSelector(string pattern, int index = 0, bool remove = false)
        {
            this.Index = new int[] { index };
            this.Pattern = pattern;
            this.Remove = remove;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pattern">pattern</param>
        /// <param name="index">results index array</param>
        /// <param name="remove"></param>
        public RegexSplitSelector(string pattern, int[] index, bool remove = true)
        {
            this.Index = index;
            this.Pattern = pattern;
            this.Remove = remove;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public RegexSplitSelector()
        {
            Pattern = "";
            Index = new int[0];
        }

        /// <summary>
        /// set selector type enum
        /// </summary>
        /// <returns>selector type</returns>
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.REGEXSPLIT;
        }

        public override string ToString()
        {
            var remove = Remove ? " -r" : "";

            if (Index.Length == 0)
            {
                return "regS /" + Pattern + "/" + remove;
            }
            else
            {
                return "regS /" + Pattern + "/ " + string.Join(" ", Index) + remove;
            }
        }
    }
}