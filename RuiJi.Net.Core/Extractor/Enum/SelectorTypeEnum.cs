using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Enum
{
    /// <summary>
    /// selector enum
    /// </summary>
    public enum SelectorTypeEnum
    {
        /// <summary>
        /// css selector
        /// </summary>
        CSS,
        /// <summary>
        /// regex selector
        /// </summary>
        REGEX,
        /// <summary>
        /// regex split selector
        /// </summary>
        REGEXSPLIT,
        /// <summary>
        /// text range selector
        /// </summary>
        TEXTRANGE,
        /// <summary>
        /// exclude selector
        /// </summary>
        EXCLUDE,
        /// <summary>
        /// no use
        /// </summary>
        INCLUDE,
        /// <summary>
        /// regex replace selector
        /// </summary>
        REGEXREPLACE,
        /// <summary>
        /// json path selector
        /// </summary>
        JPATH,
        /// <summary>
        /// xml path selector
        /// </summary>
        XPATH,
        /// <summary>
        /// clear tag selector
        /// </summary>
        CLEAR,
        /// <summary>
        /// url expression selector
        /// </summary>
        EXPRESSION,
        /// <summary>
        /// external processing function
        /// </summary>
        FUNCTION,
        /// <summary>
        /// url wildcard
        /// </summary>
        WILDCARD
    }
}