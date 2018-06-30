using Newtonsoft.Json;
using RuiJi.Net.Core.Compile;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Node.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Net.Node
{
    public class UrlCompile : ComplieBase<FileFuncProvider, JITCompile,string>
    {
        public UrlCompile()
        {
        }

        public string FormatCode(ExtractFunctionResult result)
        {
            var code = GetFunc(result.Function);

            return string.Format(code, result.Args);
        }

        public override object[] GetResult(string address)
        {
            var compileExtract = ExtractFunction(address);
            if (compileExtract == null)
                return new string[] { address };

            var reg = new Regex(@"\{#(.*?)#\}");

            var code = FormatCode(compileExtract);
            var addrs = new List<string>();

            var results = compile.GetResult(code);
            foreach (var r in results)
            {
                var addr = reg.Replace(address, r.ToString(), 1);

                var cs = GetResult(addr).Select(m=>m.ToString()).ToList();

                addrs.AddRange(cs);
            }

            return addrs.ToArray();
        }

        private ExtractFunctionResult ExtractFunction(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var regex = new Regex(@"\{#(.*?)#\}");
            var ms = regex.Matches(url);

            if (ms.Count == 0)
                return null;

            var result = new ExtractFunctionResult();
            var m = ms[0];

            var d = m.Value.Trim();
            var mt = Regex.Match(d, @"{#(.*?)\(");
            if (!mt.Success || mt.Groups.Count != 2)
                return null;

            var fun = mt.Groups[1].Value.Trim();
            var arg = Regex.Match(d, @"\((.*)\)+");

            result.Function = fun;
            result.Index = m.Index;

            if (arg.Success && arg.Groups.Count == 2)
            {
                result.Args = JsonConvert.DeserializeObject<object[]>("[" + arg.Groups[1].Value + "]");
            }

            return result;
        }
    }
}