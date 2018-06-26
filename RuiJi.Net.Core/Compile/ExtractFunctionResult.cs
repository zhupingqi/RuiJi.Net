using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    public class ExtractFunctionResult
    {
        public string Function { get; set; }

        public object[] Args { get; set; }

        public int Index { get; set; }
    }
}