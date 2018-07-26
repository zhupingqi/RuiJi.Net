using System;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Core.Compile
{
    public class JSProcessorCompile : ComplieBase<FileJsProProvider, JSCompile, KeyValuePair<string, string>>
    {
        /// <summary>
        /// get selector processor result
        /// </summary>
        /// <param name="keyValue">key:processor name \n value:processor code</param>
        /// <returns>execute result</returns>
        public override object[] GetResult(KeyValuePair<string, string> keyValue)
        {
            var name = keyValue.Key;
            var content = keyValue.Value;
            var code = GetFunc(name);

            if (string.IsNullOrEmpty(code))
                return new string[] { content };

            code = string.Format(code, content);
            return Compile.GetResult(code).ToArray();
        }
    }
}
