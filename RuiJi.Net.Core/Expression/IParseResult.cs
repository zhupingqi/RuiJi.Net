using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Expression
{
    /// <summary>
    /// ParseResult interface
    /// </summary>
    public interface IParseResult
    {
        /// <summary>
        /// if true, the parse is successful
        /// </summary>
        bool Success { get;}

        /// <summary>
        /// error message
        /// </summary>
        List<string> Messages { get; set; }

        /// <summary>
        /// result type
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// ruiji expression
        /// </summary>
        string Expression { get; set; }
    }
}