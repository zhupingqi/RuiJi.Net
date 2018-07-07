using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Net.Core.Extractor.Enum;

namespace RuiJi.Net.Core.Extractor.Selector
{
    /// <summary>
    /// json path selector
    /// </summary>
    public class JsonPathSelector : SelectorBase
    {
        /// <summary>
        /// json path
        /// </summary>
        public string JsonPath { get; set; }

        /// <summary>
        /// set selector type enum
        /// </summary>
        /// <returns>selector type</returns>
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.JPATH;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public JsonPathSelector()
        { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="path">json path</param>
        public JsonPathSelector(string path)
        {
            this.JsonPath = path;
        }
    }
}
