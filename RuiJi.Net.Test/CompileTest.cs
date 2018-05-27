using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Core.Utils;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class CompileTest
    {
        [TestMethod]
        public void TestMethod1()
        {            
            var result = JITCompile.GetResult("result = DateTime.Now.Ticks.ToString().Substring(10);");

            Assert.IsTrue(result.Length > 0);            
        }

        [TestMethod]
        public void TestRegexMatch()
        {

        }
    }
}