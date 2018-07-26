using Newtonsoft.Json;
using RuiJi.Net.Core.Compile;
using RuiJi.Net.Node.Feed.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RuiJi.Net.Node.Compile
{
    public class JSUrlCompile : ComplieBase<FileJsFuncProvider, JSCompile, string>
    { 

        private string FormatCode(UrlFunction result)
        {
            var code = GetFunc(result.Name);

            return string.Format(code, result.Args);
        }

        public override object[] GetResult(string address)
        {
            var compileExtract = ExtractFunction(address);
            if (compileExtract == null)
                return new string[] { address };

            var reg = new Regex(@"\{#(.*?)#\}");

            var code = FormatCode(compileExtract);
            if (string.IsNullOrEmpty(code))
                return new string[] { address };

            var addrs = new List<string>();

            var results = Compile.GetResult(code);
            foreach (var r in results)
            {
                var addr = reg.Replace(address, r.ToString(), 1);

                var cs = GetResult(addr).Select(m => m.ToString()).ToList();

                addrs.AddRange(cs);
            }

            return addrs.ToArray();
        }

        private UrlFunction ExtractFunction(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var regex = new Regex(@"\{#(.*?)#\}");
            var ms = regex.Matches(url);

            if (ms.Count == 0)
                return null;

            var result = new UrlFunction();
            var m = ms[0];

            var d = m.Value.Trim();
            var mt = Regex.Match(d, @"{#(.*?)\(");
            if (!mt.Success || mt.Groups.Count != 2)
                return null;

            var fun = mt.Groups[1].Value.Trim();
            var arg = Regex.Match(d, @"\((.*)\)+");

            result.Name = fun;
            result.Index = m.Index;

            if (arg.Success && arg.Groups.Count == 2)
            {
                result.Args = JsonConvert.DeserializeObject<object[]>("[" + arg.Groups[1].Value + "]");
            }

            return result;
        }

        protected override string GetFunc(string name)
        {
            var code = base.GetFunc(name);

            if (string.IsNullOrEmpty(code))
            {
                var func = FuncLiteDb.Get(name, FuncType.URLFUNCTION);
                if (func != null)
                    return func.Code;
            }

            return code;
        }
    }
}
