using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    public class ProcessorCompile : ComplieBase<FileProProvider, JITCompile,KeyValuePair<string,string>>
    {
        public override object[] GetResult(KeyValuePair<string, string> keyValue)
        {
            var name = keyValue.Key;
            var content = keyValue.Value;
            var code = GetFunc(name);

            if (string.IsNullOrEmpty(code))
                return new string[] { content };

            code = string.Format(code, content);
            return compile.GetResult(code).ToArray();
        }
    }
}