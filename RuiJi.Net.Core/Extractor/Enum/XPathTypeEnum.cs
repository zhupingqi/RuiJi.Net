using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Enum
{
    /// <summary>
    /// xml path selector enum
    /// </summary>
    public enum XPathTypeEnum
    {
        /// <summary>
        /// inner text
        /// </summary>
        TEXT,
        /// <summary>
        /// outer xml
        /// </summary>
        OUTERXML,
        /// <summary>
        /// inner xml
        /// </summary>
        INNERXML,
        /// <summary>
        /// attribute
        /// </summary>
        ATTR
    }
}
