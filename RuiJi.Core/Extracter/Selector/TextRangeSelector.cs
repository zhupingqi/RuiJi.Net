using Newtonsoft.Json;
using RuiJi.Core.Extracter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter.Selector
{
    public class TextRangeSelector : SelectorBase
    {
        [JsonIgnore]
        public new string Value { get; set; }

        [JsonProperty("begin")]
        public string Begin { get; set; }

        [JsonProperty("end")]
        public string End { get; set; }

        public TextRangeSelector(string begin, string end, RemoveEnum remove = RemoveEnum.NO)
        {
            this.Begin = begin;
            this.End = end;
            this.Remove = remove;
        }

        public TextRangeSelector()
        { }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.TEXT;
        }
    }
}