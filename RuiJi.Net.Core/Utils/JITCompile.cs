using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils
{
    public class JITCompile
    {
        private static string GenerateCode(string code)
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
                public static object v;
                
                public static void Main()
                {
                    
                }

                public static object Exec()
                {
                    object result;
                    " + code + @"
                    return result;
                }
            }";
        }

        private static CompilerResults Compile(string codes)
        {
            var compiler = new CSharpCodeProvider();
            var parameters = new CompilerParameters();
            parameters.WarningLevel = 4;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("System.dll");

            return compiler.CompileAssemblyFromSource(parameters, codes);
        }

        public static string GetResult(string code)
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

                return es;
            }

            Type type = result.CompiledAssembly.GetType("RuiJiCompile");
            return type.GetMethod("Exec").Invoke(null, new string[] { }).ToString();
        }
    }
}