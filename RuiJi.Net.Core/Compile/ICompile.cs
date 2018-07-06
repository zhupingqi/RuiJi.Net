using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    /// <summary>
    /// compile interface
    /// </summary>
    public interface ICompile
    {
        /// <summary>
        /// compile code and return result
        /// </summary>
        /// <param name="code">code</param>
        /// <returns>execute results</returns>
        List<object> GetResult(string code);

        /// <summary>
        /// compile code
        /// </summary>
        /// <param name="code">code</param>
        /// <returns>compile result</returns>
        CompilerResults Compile(string code);
    }
}