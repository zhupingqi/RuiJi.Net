using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    public interface IProvider
    {
        Dictionary<string, string> Functions { get; }

        Dictionary<string, string> LoadFuncs();

        string GetFunc(string name);
    }
}
