using NiL.JS.BaseLibrary;
using NiL.JS.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RuiJi.Net.Core.Code.Jit
{
    internal class JavascriptJitCompile : IJitCompile
    {
        static Context context;

        static JavascriptJitCompile()
        {
            context = new Context();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "misc", "proto.js");
            var proto = File.ReadAllText(path);
            context.Eval(proto);
        }

        public List<object> CompileCode(string code)
        {
            try
            {
                context.Eval(@"var results=[];" + code);

                var jsobj = context.GetVariable("results").Select(m=>m.Value.ToString()).ToList<object>();

                return jsobj;
            }
            catch (JSException e)
            {
                var errorResult = new List<object>();
                var referenceError = e.Error.Value as ReferenceError;
                if (referenceError != null)
                {
                    errorResult.Add(referenceError.ToString());
                }
                else
                {
                    errorResult.Add("Unknown error: " + e);
                }
                return errorResult;
            }
        }

        private List<object> JSObjToListObj(JSObject obj)
        {
            var result = new List<object>();
            var listJSObj = obj.Select(s => s.Value.Value).ToList<object>();
            foreach (var o in listJSObj)
            {
                if (o is JSObject)
                {
                    result.AddRange(JSObjToListObj(o as JSObject));
                }
                else
                {
                    result.Add(o);
                }
            }
            return result;
        }
    }
}
