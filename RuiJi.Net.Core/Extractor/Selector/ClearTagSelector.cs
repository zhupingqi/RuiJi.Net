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
        public string[] Tags { get; set; }

        public ClearTagSelector(string[] tags)
        {
            this.Tags = tags;
        }

        public override string ToString()
        {
            var remove = Remove ? "-r" : "";
            return string.Join(" ", "clear", Tags, remove);
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
