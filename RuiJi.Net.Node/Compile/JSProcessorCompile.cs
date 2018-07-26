using RuiJi.Net.Core.Compile;
using RuiJi.Net.Node.Feed.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Node.Compile
{
    public class JSProcessorCompile : ComplieBase<FileJsProProvider, JSCompile, KeyValuePair<string, string>>
    {
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

        protected override string GetFunc(string name)
        {
            var code = base.GetFunc(name);

            if (string.IsNullOrEmpty(code))
            {
                var func = FuncLiteDb.Get(name, FuncType.SELECTORPROCESSOR);
                if (func != null)
                    return func.Code;
            }

            return code;
        }
    }
}
