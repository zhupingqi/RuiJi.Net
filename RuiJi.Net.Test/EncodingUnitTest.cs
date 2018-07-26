using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace RuiJi.Net.Test
{
    
    public class EncodingUnitTest
    {
        [Fact]
        public void TestGB18030()
        {
            foreach (var c in System.Text.Encoding.GetEncodings())
            {
                Debug.WriteLine(c.Name);
            }

            var e = System.Text.Encoding.GetEncoding("gb18030");

            Assert.True(e != null);
        }
    }
}
