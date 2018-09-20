using Newtonsoft.Json;
using RuiJi.Net.Core.Extractor.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Selector
{
    /// <summary>
    /// selector base
    /// </summary>
    public abstract class SelectorBase : ISelector
    {
        /// <summary>
        /// value
        /// </summary>
        //[JsonProperty("value")]
        //public string Value { get; set; }

        /// <summary>
        /// remove flag
        /// </summary>
        [JsonProperty("remove")]
        public bool Remove { get; set; }

        /// <summary>
        /// selector type enum
        /// </summary>
        [JsonProperty("type")]
        public SelectorTypeEnum SelectorType { get; private set; }

        /// <summary>
        /// set selector type
        /// </summary>
        /// <returns>selector type enum</returns>
        protected abstract SelectorTypeEnum SetSelectType();

        public override abstract string ToString();

        /// <summary>
        /// constructor
        /// </summary>
        public SelectorBase()
        {
            SelectorType = SetSelectType();
        }
    }
}