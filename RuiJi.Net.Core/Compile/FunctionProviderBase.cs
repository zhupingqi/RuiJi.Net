using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    public abstract class FunctionProviderBase : IProvider
    {
        public Dictionary<string, string> Functions { get; protected set; }

        public FunctionProviderBase()
        {
            Functions = LoadFuncs();
        }

        public abstract Dictionary<string, string> LoadFuncs();

        public virtual string GetFunc(string name)
        {
            if (!Functions.Keys.Contains(name))
                return "";

            return Functions[name];
        }
    }
}
