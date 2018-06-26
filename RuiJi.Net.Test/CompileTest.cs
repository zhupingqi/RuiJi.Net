using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Node.Feed;
using RuiJi.Net.Node.Db;
using RuiJi.Net.Owin.Controllers;
using RuiJi.Net.Node;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class CompileTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var url = "http://app.cannews.com.cn/roll.php?do=query&callback=jsonp1475197217819&_={# ticks() #}&date={# now(\"yyyy-MM-dd\") #}&size=20&page={# page(1,10) #}&&start={# limit(1,5,2) #}";

            var f = new LiteDbCompileUrlProvider();
            var urls = f.Compile(url);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestSample()
        {
            var m = new FuncModel();
            m.Code = @"for (int i = 0; i < 10; i++)
            {
                result += i.ToString();
            }
            ";
            m.Sample = "timeOfDay()";

            var func = new ComplieFuncTest("result = DateTime.Now.TimeOfDay;");
            var result = func.Compile("{# " + m.Sample + " #}");

            Assert.IsTrue(result.Length > 0);
        }
    }
}