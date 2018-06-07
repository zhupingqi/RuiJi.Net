using Newtonsoft.Json;
using RuiJi.Net.Core.Extracter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extracter.Selector
{
    public class TextRangeSelector : SelectorBase
    {
        [JsonIgnore]
        public new string Value { get; set; }

        [JsonProperty("begin")]
        public string Begin { get; set; }

        [JsonProperty("end")]
        public string End { get; set; }

        public TextRangeSelector(string begin, string end, bool remove = false)
        {
            this.Begin = begin;
            this.End = end;
            this.Remove = remove;
        }

        public TextRangeSelector()
        { }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.TEXTRANGE;
        }
    }
}