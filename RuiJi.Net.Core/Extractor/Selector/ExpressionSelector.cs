using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Net.Core.Extractor.Enum;

namespace RuiJi.Net.Core.Extractor.Selector
{
    /// <summary>
    /// expression selector
    /// </summary>
    public class ExpressionSelector : SelectorBase
    {
        /// <summary>
        /// wildcard expression
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        ///  Dividing values into multiple addresses with this
        /// </summary>
        public string Split
        {
            get;
            set;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ExpressionSelector()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="split">split string</param>
        public ExpressionSelector(string expression, string split)
        {
            this.Expression = expression;
            this.Split = split;
        }

        /// <summary>
        /// set selector type
        /// </summary>
        /// <returns></returns>
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.EXPRESSION;
        }

        public override string ToString()
        {
            var remove = Remove ? " -r" : "";

            if (string.IsNullOrEmpty(Split))
            {
                return "exp " + Expression + remove;
            }

            return "exp " + Expression + " " + Split + remove;
        }
    }
}