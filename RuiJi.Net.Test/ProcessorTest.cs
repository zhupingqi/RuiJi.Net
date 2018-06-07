using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Extracter.Processor;
using RuiJi.Net.Core.Extracter.Selector;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class ProcessorTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var p = new RegexReplaceProcessor();
            var s = new RegexReplaceSelector();
            s.NewChar = ">";
            s.Value = ">>";

            var pr = new ProcessResult();
            pr.Matches.Add("评论频道>>民声");

            pr = p.ProcessNeed(s,pr);

            Assert.IsTrue(pr.Content.IndexOf(">>") == -1);
        }
    }
}
