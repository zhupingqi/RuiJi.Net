using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Cmd;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class CommandUnitTest
    {
        [TestMethod]
        public void TestCmd()
        {
            RuiJi.Net.Test.Common.StartupNodes();

            Assert.IsTrue(true);
        }
    }
}
