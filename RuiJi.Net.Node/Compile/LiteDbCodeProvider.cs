using RuiJi.Net.Core.JITCompile;
using RuiJi.Net.Node.Feed.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Node.Compile
{
    public class LiteDbCodeProvider : ICodeProvider
    {
        private FuncType funcType;

        public LiteDbCodeProvider(FuncType type)
        {
            funcType = type;
        }

        public string GetCode(string name)
        {
            var func = FuncLiteDb.Get(name, funcType);
            if (func != null)
                return func.Code;

            return "";
        }
    }
}