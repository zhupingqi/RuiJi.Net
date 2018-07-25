using RuiJi.Net.Owin.Controllers;
using System.Threading;
using Xunit;

namespace RuiJi.Net.Test
{

    public class SysUnitTest
    {
        [Fact]
        public void TestGetIfTable()
        {
            for (int i = 0; i < 10; i++)
            {
                var c = new SysInfoController();
                var info = c.SystemLoad();

                Thread.Sleep(1000);
            }

            Assert.True(true);
        }

        [Fact]
        public void TestGitPulse()
        {
            var api = new SysInfoController();
            var r = api.Pulse();

            Assert.True(r.ToString().Length > 0);
        }
    }
}
