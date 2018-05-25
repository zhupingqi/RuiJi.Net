using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Core.Extracter.Enum;

namespace RuiJi.Core.Extracter.Selector
{
    public class CssSelector : SelectorBase
    {
        [JsonProperty("attr")]
        public string AttrName { get; set; }

        [JsonProperty("ctype")]
        public CssTypeEnum Type { get; set; }

        public CssSelector(string value, CssTypeEnum type = CssTypeEnum.Text, RemoveEnum remove = RemoveEnum.NO)
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
        { }

        public CssSelector(CssSelector cssSelector)
        {
            this.AttrName = cssSelector.AttrName;
            this.Value = cssSelector.Value;
            this.Type = cssSelector.Type;
            this.Remove = cssSelector.Remove;
        }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.Css;
        }
    }
}