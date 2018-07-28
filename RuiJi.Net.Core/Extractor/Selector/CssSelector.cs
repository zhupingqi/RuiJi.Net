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
    /// css selector
    /// </summary>
    public class CssSelector : SelectorBase
    {
        [JsonProperty("selector")]
        public string Selector { get; set; }

        /// <summary>
        /// attribute name
        /// </summary>
        [JsonProperty("attr")]
        public string AttrName { get; set; }

        /// <summary>
        /// select enum
        /// </summary>
        [JsonProperty("ctype")]
        public CssTypeEnum Type { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="selector">dom selector</param>
        /// <param name="type">css type enum</param>
        /// <param name="remove">remove flag</param>
        public CssSelector(string selector, CssTypeEnum type = CssTypeEnum.TEXT, bool remove = true)
        {
            this.Selector = selector;
            this.Type = type;
            this.Remove = remove;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="selector">dom selector</param>
        /// <param name="attr">attribute name</param>
        public CssSelector(string selector, string attr)
        {
            this.Selector = selector;
            this.AttrName = attr;
            this.Type = CssTypeEnum.OUTERHTML;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public CssSelector()
        {
        }

        /// <summary>
        /// set selector type enum
        /// </summary>
        /// <returns>selector type</returns>
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.CSS;
        }
    }
}