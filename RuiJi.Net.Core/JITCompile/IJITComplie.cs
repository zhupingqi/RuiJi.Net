using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.JITCompile
{
    /// <summary>
    /// compile interface
    /// </summary>
    public interface IJITComplie
    {
        /// <summary>
        /// compile code and return result
        /// </summary>
        /// <param name="code">code</param>
        /// <returns>execute results</returns>
        List<object> CompileCode(string code);
    }
}