using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Core.Utils
{
    public class CompileExtract
    {
        public string Function { get; set; }

        public object[] Args { get; set; }
    }

    public class CompileUrl
    {
        public static CompileExtract Extract(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var regex = new Regex(@"\{#(.*?)#\}");
            var ms = regex.Matches(url);

            if (ms.Count == 0)
                return null;

            var result = new CompileExtract();

            // now("yyyyMMdd")
            // ticks()
            foreach (Match m in ms)
            {
                var d = m.Value.Trim();
                var f = Regex.Match(d,@"^[^\(]*").Value.Trim();
                var arg = Regex.Match(f, @".?\((.*)\)+");

                result.Function = f;

                if (arg.Success)
                {
                    result.Args = JsonConvert.DeserializeObject<object[]>("[" + arg.Value + "]");
                }
            }

            return result;
        }

        public static string Compile(string code)
        {
            return JITCompile.GetResult(code);
        }
    }
}