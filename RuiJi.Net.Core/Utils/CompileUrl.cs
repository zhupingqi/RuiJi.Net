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
        public List<CompileExtract> Extract(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var regex = new Regex(@"\{#(.*?)#\}");
            var ms = regex.Matches(url);

            if (ms.Count == 0)
                return null;

            var results = new List<CompileExtract>();

            // now("yyyyMMdd")
            // ticks()
            foreach (Match m in ms)
            {
                var d = m.Value.Trim();
                var mt = Regex.Match(d, @"{#(.*?)\(");
                if(!mt.Success || mt.Groups.Count != 2)
                    continue;

                var fun = mt.Groups[1].Value.Trim();
                var arg = Regex.Match(d, @"\((.*)\)+");
                var result = new CompileExtract();

                result.Function = fun;
                result.Index = m.Index;

                if (arg.Success && arg.Groups.Count == 2)
                {
                    result.Args = JsonConvert.DeserializeObject<object[]>("[" + arg.Groups[1].Value + "]");
                }

                results.Add(result);
            }

            return results;
        }

        public string Compile(string address)
        {
            var compileExtracts = Extract(address);
            var reg = new Regex(@"\{#(.*?)#\}");

            foreach (var c in compileExtracts)
            {
                var compile = "";
                var code = FormatCode(c.Function, c.Args);
                compile = JITCompile.GetResult(code);                

                address = reg.Replace(address, compile, 1);
            }

            return address;
        }

        public abstract string FormatCode(string function,object[] args);
    }
}