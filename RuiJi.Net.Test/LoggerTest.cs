using RuiJi.Net.Core.Utils.Logging;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace RuiJi.Net.Test
{
    public class LoggerTest
    {
        [Fact]
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

            Assert.True(true);
        }
    }
}
