using RuiJi.Net.Core.Code.Provider;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Core.Code.Jit
{
    public abstract class JitCompileBase : IJitCompile
    {
        public List<ICodeProvider> Providers { get; protected set; }

        public JitCompileBase()
        {
            Providers = new List<ICodeProvider>();
        }

        public abstract List<object> CompileCode(string code);

        public void AddProvider(ICodeProvider provider)
        {
            Providers.Add(provider);
        }
    }
}