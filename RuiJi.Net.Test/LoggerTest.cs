using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.Utils.Logging;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Logger.Add("main", new List<IAppender> {
                new RollingFileAppender("logs/"),
                new MemoryAppender()
            });

            Logger.GetLogger("main").Info("info");
            Logger.GetLogger("main").Error("error");
            Logger.GetLogger("main").Fatal("fatal");

            Thread.Sleep(300000);

            Assert.IsTrue(true);
        }
    }
}
