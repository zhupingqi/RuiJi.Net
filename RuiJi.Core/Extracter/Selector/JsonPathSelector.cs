using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Core.Extracter.Enum;

namespace RuiJi.Core.Extracter.Selector
{
    public class JsonPathSelector : SelectorBase
    {
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.Json;
        }
    }
}
