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
    /// xml path selector
    /// </summary>
    public class XPathSelector : SelectorBase
    {
        /// <summary>
        /// xpath
        /// </summary>
        public string XPath { get; set; }

        /// <summary>
        /// attribute name
        /// </summary>
        [JsonProperty("attr")]
        public string AttrName { get; set; }

        /// <summary>
        /// select type
        /// </summary>
        [JsonProperty("xtype")]
        public XPathTypeEnum Type { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public XPathSelector()
        { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="xpath">xpath</param>
        /// <param name="attr">attribute name</param>
        public XPathSelector(string xpath, string attr)
        {
            this.XPath = xpath;
            this.AttrName = attr;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="xpath">xpath</param>
        /// <param name="type">select type</param>
        /// <param name="remove">remove flag</param>
        public XPathSelector(string xpath, XPathTypeEnum type = XPathTypeEnum.TEXT, bool remove = true)
        {
            this.XPath = xpath;
            this.Type = type;
            this.Remove = remove;
        }

        /// <summary>
        /// set selector type enum
        /// </summary>
        /// <returns>selector type</returns>
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.XPATH;
        }
    }
}