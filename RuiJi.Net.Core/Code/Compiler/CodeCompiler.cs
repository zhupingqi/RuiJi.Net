using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RuiJi.Net.Core.Code.Compiler
{
    public class CodeCompiler : CodeCompilerBase
    {
        public CodeCompiler(string language = "javascript") : base(language)
        {

        }

        /// <summary>
        /// execute function from url
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>execute result</returns>
        public override object[] GetResult(params object[] p)
        {
            var name = p[0].ToString();
            var content = p[1].ToString();
            var code = GetCode(name);

            if (string.IsNullOrEmpty(code))
                return new string[] { content };

            return JITCompile.CompileCode(code).ToArray();
        }

        public override object[] Test(string sample, string code)
        {
            code = "var content = \"" + sample + "\";\r\n" + code;

            return JITCompile.CompileCode(code).ToArray();
        }
    }
}