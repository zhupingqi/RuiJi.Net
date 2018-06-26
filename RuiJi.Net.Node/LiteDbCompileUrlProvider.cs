using RuiJi.Net.Core.Compile;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Node.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node
{
    public class LiteDbCompileUrlProvider : FileCompileUrlProvider
    {
        public override string FormatCode(ExtractFunctionResult result)
        {
            var code = base.FormatCode(result);

            if(string.IsNullOrEmpty(code))
            {
                var f = FuncLiteDb.Get(result.Function);
                if (f != null)
                {
                    code = string.Format(f.Code, result.Args);
                }
            }

            return code;
        }
    }
}