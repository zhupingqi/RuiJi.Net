using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    public abstract class ComplieBase<T1,T2,T3> where T1 : IProvider, new() where T2 : ICompile, new()
    {
        public T1 provider { get; set; }

        public T2 compile { get; set; }

        public ComplieBase()
        {
            provider = new T1();
            compile = new T2();
        }

        public virtual string GetFunc(string name)
        {
            return provider.GetFunc(name);
        }

        public abstract object[] GetResult(T3 t);
    }
}