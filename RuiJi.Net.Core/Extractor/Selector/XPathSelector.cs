using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Net.Core.Extractor.Enum;

namespace RuiJi.Net.Core.Extractor.Selector
{
    public class XPathSelector : SelectorBase
    {
        [JsonProperty("attr")]
        public string AttrName { get; set; }

        [JsonProperty("xtype")]
        public XPathTypeEnum Type { get; set; }

        public XPathSelector()
        { }

        public XPathSelector(string value, string attr)
        {
            this.Value = value;
            this.AttrName = attr;
        }

        public XPathSelector(string value, XPathTypeEnum type = XPathTypeEnum.InnerText, bool remove = true)
        {
            this.Value = value;
            this.Type = type;
            this.Remove = remove;
        }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.XPATH;
        }
    }
}