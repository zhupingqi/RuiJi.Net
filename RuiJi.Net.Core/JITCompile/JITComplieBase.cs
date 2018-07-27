using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Core.JITCompile
{
    public abstract class JITComplieBase : IJITComplie
    {
        public List<ICodeProvider> Providers { get; protected set; }

        public JITComplieBase()
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