using RuiJi.Net.Core.Code.Compiler;
using RuiJi.Net.Core.Code.Provider;
using RuiJi.Net.Node.CodeProvider;
using RuiJi.Net.Node.Feed.Db;
using System.Collections.Generic;
using Xunit;

namespace RuiJi.Net.Test
{
    public class CompileTest
    {
        [Fact]
        public void TestMethod1()
        {
            var url = "http://www.onezh.com/hall/show_{# page(146,707) #}.html";

            CodeCompilerManager.Create("url", new List<ICodeProvider> {
                    new LiteDbCodeProvider(Node.Feed.Db.FuncType.URLFUNCTION),
                    new FileCodeProvider("funcs/js","fun")
                });

            CodeCompilerManager.Create("proc", new List<ICodeProvider> {
                    new LiteDbCodeProvider(Node.Feed.Db.FuncType.SELECTORPROCESSOR),
                    new FileCodeProvider("funcs/js","pro")
                });
            var addrs = CodeCompilerManager.GetResult("url", url);

            Assert.True(true);
        }

        //[Fact]
        //public void TestSample()
        //{
        //    var m = new FuncModel();
        //    m.Code = @"for (var i = 0; i < 10; i++)
        //    {
        //        result += i.ToString();
        //    }
        //    ";
        //    m.Sample = "timeOfDay()";

        //    var func = new ComplieFuncTest("result = DateTime.Now.TimeOfDay;");
        //    var result = func.GetResult("{# " + m.Sample + " #}");

        //    Assert.True(result.Length > 0);
        //}
    }
}