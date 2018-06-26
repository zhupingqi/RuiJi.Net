using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Expression
{
    public class ParseResult<T> : IParseResult where T : new()
    {
        public bool Success
        {
            get
            {
                return Messages.Count == 0;
            }
        }

        public T Result { get; set; }

        public List<string> Messages { get; set; }

        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        public string Expression { get; set; }

        public ParseResult(string expression)
        {
            Result = new T();

            Messages = new List<string>();

            this.Expression = expression;
        }
    }
}