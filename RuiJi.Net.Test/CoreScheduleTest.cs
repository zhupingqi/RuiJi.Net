using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.RTS;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class CoreScheduleTest
    {
        [TestMethod]
        public void TestFeedScheduler()
        {
            var s = typeof(int).Name;

            FeedScheduler.Start();
            FeedScheduler.AddJob("1", "0/5 * * * * ?");
            FeedScheduler.AddJob("1", "0/10 * * * ?");

            Thread.Sleep(3000000);
            Assert.IsTrue(true);
        }
    }
}
