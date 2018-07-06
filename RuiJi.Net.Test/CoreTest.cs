using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.RTS;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class CoreTest
    {
        [TestMethod]
        public void TestFeedScheduler()
        {
            FeedScheduler.Start("0/5 * * * * ?");

            Thread.Sleep(3000000);
            Assert.IsTrue(true);
        }
    }
}
