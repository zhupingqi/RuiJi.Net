using NiL.JS.BaseLibrary;
using NiL.JS.Core;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    public class JSCompile : ICompile
    {
        static Context context;

        static JSCompile()
        {
            context = new Context();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "misc", "proto.js");
            var proto = File.ReadAllText(path);
            context.RootContext.Eval(proto);
        }

        public JSCompile()
        {
            
        }

        public CompilerResults Compile(string code)
        {
            throw new NotImplementedException();
        }

        public List<object> GetResult(string code)
        {
            try
            {
                context.Eval(@"var results=[];" + code);

                var jsobj = context.GetVariable("results").ToObject();

                return JSObjToListObj(jsobj);
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
