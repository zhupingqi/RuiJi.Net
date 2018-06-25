using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Net.Core.Extractor.Enum;

namespace RuiJi.Net.Core.Extractor.Selector
{
    public class CssSelector : SelectorBase
    {
        [JsonProperty("attr")]
        public string AttrName { get; set; }

        [JsonProperty("ctype")]
        public CssTypeEnum Type { get; set; }

        public CssSelector(string value, CssTypeEnum type = CssTypeEnum.Text, bool remove = true)
        {
            this.Value = value;
            this.Type = type;
            this.Remove = remove;
        }

        public CssSelector(string value, string attr)
        {
            this.Value = value;
            this.AttrName = attr;
            this.Type = CssTypeEnum.OutHtml;
        }

        public CssSelector()
        {

        }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.CSS;
        }
    }
}