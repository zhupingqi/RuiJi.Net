using RuiJi.Net.Core.Code.Provider;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Core.Code.Compiler
{
    public class CodeCompilerManager
    {
        private static Dictionary<string, CodeCompilerBase> compilers;

        static CodeCompilerManager() => compilers = new Dictionary<string, CodeCompilerBase>();

        public static CodeCompilerBase Create(string name, List<ICodeProvider> providers, string language = "javascript")
        {
            CodeCompilerBase compiler = null;

            switch (name)
            {
                case "url":
                    {
                        compiler = new UrlCodeCompiler(language);
                        break;
                    }
                case "proc":
                    {
                        compiler = new CodeCompiler(language);
                        break;
                    }
            }

            compiler.Providers = providers;

            compilers.Add(name, compiler);

            return compiler;
        }

        public static object[] GetResult(string name, params object[] p)
        {
            if (compilers.ContainsKey(name))
                return compilers[name].GetResult(p);

            return p;
        }

        public static object[] Test(string name, string sample,string code)
        {
            if (compilers.ContainsKey(name))
                return compilers[name].Test(sample, code);

            return new object[] { sample };
        }
    }
}