using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Net.Core.Extractor.Enum;

namespace RuiJi.Net.Core.Extractor.Selector
{
    /// <summary>
    /// clear tag selector
    /// </summary>
    public class ClearTagSelector : SelectorBase
    {
        public ClearTagSelector()
        {            
        }

        /// <summary>
        /// set selector type enum
        /// </summary>
        /// <returns></returns>
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.CLEAR;
        }
    }
}
