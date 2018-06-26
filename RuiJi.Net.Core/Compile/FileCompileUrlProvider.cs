using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    public class FileCompileUrlProvider : CompileUrlProviderBase
    {
        private static Dictionary<string, string> functions = new Dictionary<string, string>();

        static FileCompileUrlProvider()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funcs");

            foreach (var file in Directory.GetFiles(path))
            {
                var key = file.Substring(file.LastIndexOf(@"\") + 1).Replace(".fun","");
                var func = File.ReadAllText(file);

                functions.Add(key, func);
            }
        }

        public override string FormatCode(ExtractFunctionResult result)
        {
            if (!functions.Keys.Contains(result.Function))
                return "";

            var code = functions[result.Function];

            return string.Format(code, result.Args);
        }
    }
}