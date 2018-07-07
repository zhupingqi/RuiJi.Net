using RuiJi.Net.Core.Extractor.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Selector
{
    /// <summary>
    /// selector interface
    /// </summary>
    public interface ISelector
    {
        /// <summary>
        /// selector type
        /// </summary>
        SelectorTypeEnum SelectorType { get; }

        /// <summary>
        /// remove flag
        /// </summary>
        bool Remove { get; set; }

        /// <summary>
        /// selector value
        /// </summary>
        //string Value { get; set; }
    }
}