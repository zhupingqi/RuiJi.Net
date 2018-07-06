using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    /// <summary>
    /// url compile class,extract url function from url and execute function
    /// </summary>
    public class UrlCompile : ComplieBase<FileFuncProvider, JITCompile,string>
    {
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
        protected virtual string FormatCode(UrlFunction result)
        {
            var code = GetFunc(result.Name);

            return string.Format(code, result.Args);
        }

        /// <summary>
        /// execute function from url
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>execute result</returns>
        public override object[] GetResult(string url)
        {
            var compileExtract = ExtractFunction(url);
            if (compileExtract == null)
                return new string[] { url };

            var reg = new Regex(@"\{#(.*?)#\}");

            var code = FormatCode(compileExtract);
            var addrs = new List<string>();
            var results = Compile.GetResult(code);

            foreach (var r in results)
            {
                var addr = reg.Replace(url, r.ToString(), 1);

                var cs = GetResult(addr).Select(m=>m.ToString()).ToList();

                addrs.AddRange(cs);
            }

            return addrs.ToArray();
        }
    }
}