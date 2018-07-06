using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    /// <summary>
    /// C# compile
    /// </summary>
    public class JITCompile : ICompile
    {
        /// <summary>
        /// generate code
        /// </summary>
        /// <param name="code">code</param>
        /// <returns>merged code</returns>
        private string GenerateCode(string code)
        {
            return @"
            using System;
            using System.ComponentModel;
            using System.Text;
            using System.Text.RegularExpressions;
            using System.Collections;
            using System.Collections.Generic;
            using System.Reflection;

            sealed class RuiJiCompile
            {                
                public static void Main()
                {
                    
                }

                public static List<object> Exec()
                {
                    var results = new List<object>();
                    " + code + @"
                    return results;
                }
            }";
        }

        /// <summary>
        /// compile code
        /// </summary>
        /// <param name="code">code</param>
        /// <returns>compile result</returns>
        public CompilerResults Compile(string code)
        {
            var compiler = new CSharpCodeProvider();
            var parameters = new CompilerParameters();
            parameters.WarningLevel = 4;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("System.dll");

            return compiler.CompileAssemblyFromSource(parameters, code);
        }

        /// <summary>
        /// execute code 
        /// </summary>
        /// <param name="code">code</param>
        /// <returns>execute results</returns>
        public List<object> GetResult(string code)
        {
            code = GenerateCode(code);
            var result = Compile(code);

            if(result.Errors.HasErrors)
            {
                var es = "";
                foreach (CompilerError er in result.Errors)
                {
                    es += er.ErrorText;
                }

                return new List<object> { es };
            }

            Type type = result.CompiledAssembly.GetType("RuiJiCompile");
            return (type.GetMethod("Exec").Invoke(null, new string[] { }) as List<object>).ToList();
        }
    }
}