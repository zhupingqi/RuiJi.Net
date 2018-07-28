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
    /// function selector
    /// </summary>
    public class FunctionSelector : SelectorBase
    {
        /// <summary>
        /// function name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public FunctionSelector()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">function name</param>
        public FunctionSelector(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// set selector type enum
        /// </summary>
        /// <returns>selector type</returns>
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.FUNCTION;
        }
    }
}