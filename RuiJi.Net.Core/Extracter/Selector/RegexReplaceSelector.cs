using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Net.Core.Extracter.Enum;

namespace RuiJi.Net.Core.Extracter.Selector
{
    public class RegexReplaceSelector : SelectorBase
    {
        [JsonProperty("newChar")]
        public string NewChar { get; set; }

        public RegexReplaceSelector()
        {

        }

        public RegexReplaceSelector(string value, string newChar)
        {
            this.NewChar = newChar;
            this.Value = value;
        }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.REPLACE;
        }
    }
}
