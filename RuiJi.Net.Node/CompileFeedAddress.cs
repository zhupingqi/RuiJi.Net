using RuiJi.Net.Core.Utils;
using RuiJi.Net.Node.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node
{
    public class CompileFeedAddress : CompileUrl
    {
        public override string FormatCode(CompileExtract extract)
        {
            var code = "";

            switch (extract.Function)
            {
                case "now":
                    {
                        code = string.Format("results.Add(DateTime.Now.ToString(\"{0}\"));", extract.Args);
                        break;
                    }
                case "ticks":
                    {
                        code = string.Format("results.Add(DateTime.Now.Ticks);", extract.Args);
                        break;
                    }
                case "page":
                    {
                        code = string.Format(@"for (int i = {0}; i <= {1}; i++){{results.Add(i);}}"
                            , extract.Args);
                        break;
                    }
                case "limit":
                    {
                        code = string.Format(@"for (int i = {0}; i <= {1}; i++){{results.Add((i-1)*{2});}}"
                            , extract.Args);
                        break;
                    }
                default:
                    {
                        var f = FuncLiteDb.Get(extract.Function);
                        if (f != null)
                        {
                            code = string.Format(f.Code, extract.Args);
                        }
                        break;
                    }
            }

            return code;
        }
    }
}
