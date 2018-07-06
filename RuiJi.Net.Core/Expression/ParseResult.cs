using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Expression
{
    /// <summary>
    /// ParseResult
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    public class ParseResult<T> : IParseResult where T : IRuiJiParseResult, new()
    {
        /// <summary>
        /// if true, the parse is successful
        /// </summary>
        public bool Success
        {
            get
            {
                return Messages.Count == 0;
            }
        }

        /// <summary>
        /// result type
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// error message
        /// </summary>
        public List<string> Messages { get; set; }

        /// <summary>
        /// result type
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// ruiji expression
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="expression">ruiji expression</param>
        public ParseResult(string expression)
        {
            Result = new T();

            Messages = new List<string>();

            this.Expression = expression;
        }
    }
}