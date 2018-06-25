using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Net.Core.Extractor.Enum;

namespace RuiJi.Net.Core.Extractor.Selector
{
    public class ClearTagSelector : SelectorBase
    {
        public ClearTagSelector()
        {
            
        }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.CLEAR;
        }
    }
}
