using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    public interface ICompile
    {
        List<object> GetResult(string code);

        CompilerResults Compile(string codes);
    }
}