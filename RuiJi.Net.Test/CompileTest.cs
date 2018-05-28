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
            var code = string.Format("result = DateTime.Now.ToString(\"{0}\");","yyyy");

            var result = JITCompile.GetResult(code);

            Assert.IsTrue(result.Length > 0);            
        }

        [TestMethod]
        public void TestRegexMatch()
        {

        }
    }
}