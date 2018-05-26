using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Core.Extracter.Enum;

namespace RuiJi.Core.Extracter.Selector
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
