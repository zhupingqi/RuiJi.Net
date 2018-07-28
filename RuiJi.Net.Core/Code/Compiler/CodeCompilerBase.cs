using RuiJi.Net.Core.Code.Jit;
using RuiJi.Net.Core.Code.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Core.Code.Compiler
{
    public abstract class CodeCompilerBase
    {
        public List<ICodeProvider> Providers { get; set; }

        protected IJitCompile JITCompile { get; set; }

        public CodeCompilerBase(string language)
        {
            Providers = new List<ICodeProvider>();

            if (language == "javascript")
            {
                JITCompile = new JavascriptJitCompile();
            }

            if (language == "csharp")
            {
                JITCompile = new SharpJitCompile();
            }
        }

        public void AddProvider(ICodeProvider provider)
        {
            Providers.Add(provider);
        }

        /// <summary>
        /// get function code by function name
        /// </summary>
        /// <param name="name">function name</param>
        /// <returns>function code body</returns>
        public string GetCode(string name)
        {
            foreach (var provider in Providers)
            {
                var code = provider.GetCode(name);

                if (!string.IsNullOrEmpty(code))
                    return code;
            }

            return "";
        }

        public abstract object[] GetResult(params object[] p);

        public abstract object[] Test(string sample,string code);
    }
}