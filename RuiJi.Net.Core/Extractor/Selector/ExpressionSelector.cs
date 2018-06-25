using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Net.Core.Extractor.Enum;

namespace RuiJi.Net.Core.Extractor.Selector
{
    public class ExpressionSelector : SelectorBase
    {
        public string Split
        {
            get;
            set;
        }

        public ExpressionSelector()
        {

        }

        public ExpressionSelector(string value,string split)
        {
            this.Value = value;
            this.Split = split;
        }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.EXPRESSION;
        }
    }
}