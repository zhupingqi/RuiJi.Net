using System;
using System.Threading;
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
            for (int i = 0; i < 10; i++)
            {
                var c = new InfoApiController();
                var info = c.System();

                Thread.Sleep(1000);
            }

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestGitPulse()
        {
            var api = new InfoApiController();
            var r = api.Pulse();

            Assert.IsTrue(r.ToString().Length > 0);
        }
    }
}
