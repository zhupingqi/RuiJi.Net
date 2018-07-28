using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RuiJi.Net.Core.Code.Compiler
{
    public class UrlCodeCompiler : CodeCompilerBase
    {
        public UrlCodeCompiler(string language) : base(language)
        {
        }

        /// <summary>
        /// extract url function from url
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>url function</returns>
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

        /// <summary>
        /// get url function code and format code
        /// </summary>
        /// <param name="result">url function</param>
        /// <returns>formated code</returns>
        private string FormatCode(UrlFunction result)
        {
            var code = GetCode(result.Name);

            return string.Format(code, result.Args);
        }

        public override object[] GetResult(params object[] p)
        {
            var url = p[0].ToString();
            var func = ExtractFunction(url);
            if (func == null)
                return new string[] { url };

            var code = FormatCode(func);

            var addrs = new List<string>();
            var results = JITCompile.CompileCode(code);
            var reg = new Regex(@"\{#(.*?)#\}");

            foreach (var r in results)
            {
                var addr = reg.Replace(url, r.ToString(), 1);

                var cs = GetResult(addr).Select(m => m.ToString()).ToList();

                addrs.AddRange(cs);
            }

            return addrs.ToArray();
        }

        public override object[] Test(string sample, string code)
        {
            var s = "{# " + sample + " #}";

            var func = ExtractFunction(s);
            if (func == null)
                return new string[] { sample };

            code = string.Format(code, func.Args);

            var addrs = new List<string>();
            var results = JITCompile.CompileCode(code);
            var reg = new Regex(@"\{#(.*?)#\}");

            foreach (var r in results)
            {
                var addr = reg.Replace(s, r.ToString(), 1);

                var cs = GetResult(addr).Select(m => m.ToString()).ToList();

                addrs.AddRange(cs);
            }

            return addrs.ToArray();
        }

        class UrlFunction
        {
            /// <summary>
            /// function name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// function args
            /// </summary>
            public object[] Args { get; set; }

            /// <summary>
            /// function index at url
            /// </summary>
            public int Index { get; set; }
        }
    }
}
