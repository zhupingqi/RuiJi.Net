using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Tasks;
using RuiJi.Net.Node.Feed;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.Owin.Controllers;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class ExpressionUnitTest
    {
        [TestMethod]
        public void TestMeta()
        {
            var metas =
                @"
                #css
                css h4 a[href] -r
                css h4:ohtml
                css h4:html -r
                css h4 a:text

                #exclude
                ex /:/ -b
                ex /\-e/ -e
                ex /\-/ -a
                ex /[\d]*/

                #expression
                exp ????/??/?? ??:??:??* 
                exp datetime_?? -r

                #regex
                reg /[\d]*/
                reg /aa([\d]*)sf/ 0 1
                reg /aa([\d]*)sf/ -r

                #regexReplace
                regR /aaaa/ dddd/
                
                #regexSplit
                regS /aaa/ 2 3 5
                regS /aaa/ 2 3 5 -r

                #textRange
                text /a\naa/ /b\tbb/
                text /aaa/ /bbb/ -r

                #xpath
                xpath ladkfeio
                xpath dlqwekrjl -r

                #jsonPath
                jpath dlsldf.kljs
                jpath dlkejl -r
                ";

            var m = RuiJiExpression.ParserMeta(metas);

            Assert.IsTrue(m.Count > 0);
        }

        [TestMethod]
        public void TestBlock()
        {
            var block = @"
[block]
#name
css .entry-content:html

[blocks]
    @block1
    @block2

[tile]
    #aa
    css a:ohtml

    [meta]
    #time
    css time:text

[meta]
    #time
    css time:text

    #author
    css .author:text

    #title
    css .entry-title:text

    #content
    css .entry-content:html

    #link
    css h4 a[href] -r
[block]
#block1
css .list1

[block]
#block2
css .list2
";

            var m = RuiJiExpression.ParserBlock(block);

            Assert.IsTrue(m.Metas.Count > 0);
        }

        [TestMethod]
        public void TestPaging()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("https://www.kuaidaili.com/free/inha/1/");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var block = new ExtractBlock();
            var s = RuiJiExpression.ParserBlock(@"
[tile]
	css table.table-bordered tr:gt(0):ohtml

	[meta]
	#ip
	css td[data-title='IP']:text

    # port
    css td[data-title='PORT']:text

[paging]
css #listnav a[href]
");

            var result = RuiJiExtracter.Extract(content, s);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestExtract()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.ruijihg.com/%e5%bc%80%e5%8f%91/");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var block = new ExtractBlock();
            var s = RuiJiExpression.ParserBase("css a[href]").Selectors;
            block.TileSelector.Selectors.AddRange(s);
            var result = RuiJiExtracter.Extract(content, block);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestHistoryUpdate()
        {
            var feed = FeedLiteDb.GetFeed(1);
            var job = new FeedExtractJob();
            job.DoTask(@"D:\云同步\vcoded\RuiJi.Net\RuiJi.Cmd\bin\Debug\snapshot\1_636635303414097356.json");

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestUrlExtract()
        {
            var result = ContentQueue.Instance.Extract("http://www.ruijihg.com/archives/315");

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCrawlTaskFunc()
        {
            var task = new ParallelTask();
            var model = new CrawlTaskModel();
            model.FeedId = 3;

            var fun = new CrawlTaskFunc();
            var result = fun.Run(model, task);

            Assert.IsTrue(result != null);
        }
    }
}
