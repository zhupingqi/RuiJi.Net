using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Core.Extracter.Enum;

namespace RuiJi.Core.Extracter.Selector
{
    public class RegexSplitSelector : SelectorBase
    {
        [JsonProperty("index")]
        public int[] Index { get; set; }

        public RegexSplitSelector(string value, int index = 0, RemoveEnum remove = RemoveEnum.NO)
        {
            this.Index = new int[] { index };
            this.Value = value;
            this.Remove = remove;
        }

        public RegexSplitSelector(string value, int[] index, RemoveEnum remove = RemoveEnum.NO)
        {
            this.Index = index;
            this.Value = value;
            this.Remove = remove;
        }

        public RegexSplitSelector()
        { }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.RegexSplit;
        }
    }
}