using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RuiJi.Net.Core.JITCompile
{
    public class Compiler
    {
        public List<ICodeProvider> Providers { get; set; }

        protected IJITComplie JITCompile { get; private set; }

        public Compiler(string language = "javascript")
        {
            Providers = new List<ICodeProvider>();

            if (language == "javascript")
            {
                JITCompile = new JavascriptJITCompile();
            }

            if (language == "csharp")
            {
                JITCompile = new SharpJITCompile();
            }
        }

        public void AddProvider(ICodeProvider provider)
        {
            Providers.Add(provider);
        }

        /// <summary>
        /// get function code by function name
        /// </summary>
        /// <param name="name">function name</param>
        /// <returns>function code body</returns>
        public string GetCode(string name)
        {
            foreach (var provider in Providers)
            {
                var code = provider.GetCode(name);

                if (!string.IsNullOrEmpty(code))
                    return code;
            }

            return "";
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
        protected virtual string FormatCode(UrlFunction result)
        {
            var code = GetCode(result.Name);

            return string.Format(code, result.Args);
        }

        /// <summary>
        /// execute function from url
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>execute result</returns>
        public object[] GetResult(string type,params object[] p)
        {
            if (type == "url")
            {
                var url = p[0].ToString();
                var code = "";

                if (p.Length == 2)
                {
                    code = p[1].ToString();
                }
                else
                {
                    var compileExtract = ExtractFunction(url);
                    if (compileExtract == null)
                        return new string[] { url };

                    code = FormatCode(compileExtract);
                }

                var addrs = new List<string>();
                var results = JITCompile.CompileCode(code);
                var reg = new Regex(@"\{#(.*?)#\}");

                foreach (var r in results)
                {
                    var addr = reg.Replace(url, r.ToString(), 1);

                    var cs = GetResult(type, addr).Select(m => m.ToString()).ToList();

                    addrs.AddRange(cs);
                }

                return addrs.ToArray();
            }
            else
            {
                var name = p[0].ToString();
                var content = p[1].ToString();
                var code = GetCode(name);

                if (string.IsNullOrEmpty(code))
                    return new string[] { content };

                code = string.Format(code, content);
                return JITCompile.CompileCode(code).ToArray();
            }
        }
    }
}