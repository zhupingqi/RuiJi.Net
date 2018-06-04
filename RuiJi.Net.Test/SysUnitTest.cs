using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Owin.Controllers;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class SysUnitTest
    {
        [TestMethod]
        public void TestGetIfTable()
        {
            while (true)
            {
                var c = new InfoApiController();
                var info = c.System();
            }

            Assert.IsTrue(true);
        }
    }
}
