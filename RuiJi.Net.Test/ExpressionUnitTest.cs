using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Tasks;
using RuiJi.Net.Node.Feed;
using RuiJi.Net.Node.Db;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Owin.Controllers;
using RuiJi.Net.Node;

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
                regR />>/ >
                
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
            var result = Cooperater.GetResult("http://www.cannews.com.cn/2018/0606/177699.shtml");

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

        [TestMethod]
        public void TestJsonPExtract()
        {
            var url = "http://app.cannews.com.cn/roll.php?do=query&callback=jsonp1475197217819&_={# ticks() #}&date={# now(\"yyyy-MM-dd\") #}&size=20&page=1";

            var f = new CompileFeedAddress();
            //url = f.Compile(url);

            var c = new RuiJiCrawler();
            var response = c.Request(new Request(url));

            var expression = @"
reg /jsonp[\d]+?\((.*)\)/ 1
jpath $..url
";
            var b = RuiJiExpression.ParserBlock(expression);
            var result = RuiJiExtracter.Extract(response.Data.ToString(), b);

            Assert.IsTrue(result.Content.ToString().Length > 0);
        }

        [TestMethod]
        public void TestFeature()
        {

        }

        [TestMethod]
        public void TestExpressionType()
        {
            var block = @"
[block]
#name_dda_ee
css .entry-content:html

[blocks]
    @block1
    @block2

[tile]
    #aa_l
    css a:ohtml

    [meta]
        #time_dt
        css time:text

[meta]
    #time_dt
    css time:text

    #words_i
    css .author:text

    #score_d
    css .entry-title:text

    #score_1_f
    css .entry-content:html

    #hasLink_b
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
    }
}
