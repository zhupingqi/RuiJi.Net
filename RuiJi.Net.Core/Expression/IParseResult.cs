using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Expression
{
    public interface IParseResult
    {
        bool Success{ get;}

        List<string> Messages { get; set; }

        Type Type { get; }

        string Expression { get; set; }
    }
}