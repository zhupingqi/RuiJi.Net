using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Owin.Controllers;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class CompileTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var url = "http://app.cannews.com.cn/roll.php?do=query&callback=jsonp1475197217819&_={# ticks() #}&date={# now(\"yyyy-MM-dd\") #}&size=20&page=1";

            var f = new CompileFeedAddress();
            url = f.Compile(url);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestRegexMatch()
        {

        }
    }
}