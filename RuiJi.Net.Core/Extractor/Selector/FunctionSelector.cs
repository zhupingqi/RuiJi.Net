using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Net.Core.Extractor.Enum;

namespace RuiJi.Net.Core.Extractor.Selector
{
    public class FunctionSelector : SelectorBase
    {
        public FunctionSelector()
        {

        }

        public FunctionSelector(string vale)
        {
            this.Value = vale;
        }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.FUNCTION;
        }
    }
}
