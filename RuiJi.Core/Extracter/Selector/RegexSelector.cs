using Newtonsoft.Json;
using RuiJi.Core.Extracter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter.Selector
{
    public class RegexSelector : SelectorBase
    {
        [JsonProperty("index")]
        public int[] Index { get; set; }

        public RegexSelector(string value, int index = 0, RemoveEnum remove = RemoveEnum.NO)
        {
            this.Index = new int[] { index };
            this.Value = value;
            this.Remove = remove;
        }

        public RegexSelector(string value, int[] index, RemoveEnum remove = RemoveEnum.NO)
        {
            this.Index = index;
            this.Value = value;
            this.Remove = remove;
        }

        public RegexSelector()
        { }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.Regex;
        }
    }
}
