using Newtonsoft.Json;
using RuiJi.Net.Core.Extractor.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Selector
{
    /// <summary>
    /// text range selector
    /// </summary>
    public class TextRangeSelector : SelectorBase
    {
        /// <summary>
        /// text begin regex pattern
        /// </summary>
        [JsonProperty("begin")]
        public string Begin { get; set; }

        /// <summary>
        /// text end regex pattern
        /// </summary>
        [JsonProperty("end")]
        public string End { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="begin">text begin pattern</param>
        /// <param name="end">text end pattern</param>
        /// <param name="remove"></param>
        public TextRangeSelector(string begin, string end, bool remove = false)
        {
            this.Begin = begin;
            this.End = end;
            this.Remove = remove;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public TextRangeSelector()
        { }

        /// <summary>
        /// set selector type enum
        /// </summary>
        /// <returns>selector type</returns>
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.TEXTRANGE;
        }

        public override string ToString()
        {
            var remove = Remove ? " -r" : "";
            return "text /" + Begin + "/ /" + End + "/" + remove;
        }
    }
}