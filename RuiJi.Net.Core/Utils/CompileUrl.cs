using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils
{
    public class CompileExtract
    {
        public string Function { get; set; }

        public object[] Args { get; set; }

        public int Index { get; set; }
    }

    public abstract class CompileUrl
    {
        public CompileExtract Extract(string url)
        {

            if (string.IsNullOrEmpty(url))
                return null;

            var regex = new Regex(@"\{#(.*?)#\}");
            var ms = regex.Matches(url);

            if (ms.Count == 0)
                return null;

            var result = new CompileExtract();
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

        public string[] Compile(string address)
        {
            var compileExtract = Extract(address);
            if (compileExtract == null)
                return new string[] { address };

            var reg = new Regex(@"\{#(.*?)#\}");

            var code = FormatCode(compileExtract);
            var addrs = new List<string>();

            var results = JITCompile.GetResult(code);
            foreach (var r in results)
            {
                var addr = reg.Replace(address, r, 1);

                var cs = Compile(addr);

                addrs.AddRange(cs);
            }

            return addrs.ToArray();
        }

        public abstract string FormatCode(CompileExtract compile);
    }
}