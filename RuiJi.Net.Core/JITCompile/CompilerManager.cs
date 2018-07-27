using System;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Core.JITCompile
{
    public class CompilerManager
    {
        private static Dictionary<string, Compiler> compilers;

        static CompilerManager() => compilers = new Dictionary<string, Compiler>();

        public static Compiler Create(string name, List<ICodeProvider> providers, string language = "javascript")
        {
            var compiler = new Compiler(language);
            compiler.Providers = providers;

            compilers.Add(name, compiler);

            return compiler;
        }

        public static object[] GetResult(string name, params object[] p)
        {
            if (compilers.ContainsKey(name))
                return compilers[name].GetResult(name, p);

            return p;
        }
    }
}