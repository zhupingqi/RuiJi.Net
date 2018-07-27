using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuiJi.Net.Core.JITCompile
{
    internal class SharpJITCompile : IJITComplie
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
            using System.Collections.Generic;

            public class RuiJiCompile
            {                
                public static List<object> Exec()
                {
                    List<object> results = new List<object>();
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
        private CompilerResults CompileSharpCode(string code)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var cp = new CompilerParameters();
            //cp.GenerateExecutable = true;
            cp.GenerateInMemory = true;
            cp.TreatWarningsAsErrors = false;
            cp.IncludeDebugInformation = true;
            cp.MainClass = "RuiJiCompile";
            //cp.ReferencedAssemblies.Add("System.dll");

            return provider.CompileAssemblyFromSource(cp, code);
        }

        /// <summary>
        /// execute code 
        /// </summary>
        /// <param name="code">code</param>
        /// <returns>execute results</returns>
        public List<object> CompileCode(string code)
        {
            code = GenerateCode(code);
            var result = CompileSharpCode(code);

            if (result.Errors.HasErrors)
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
