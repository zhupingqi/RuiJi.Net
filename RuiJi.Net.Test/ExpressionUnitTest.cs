using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Utils.Tasks;
using RuiJi.Net.Node.Feed.Db;
using RuiJi.Net.Node.Feed.LTS;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Owin.Models;
using System;
using System.IO;
using Xunit;

namespace RuiJi.Net.Test
{
    public class ExpressionUnitTest
    {
        [Fact]
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

            var m = RuiJiBlockParser.ParserMeta(metas);

            Assert.True(m.Count > 0);
        }

        [Fact]
        public void TestBlock()
        {
            var block = @"
[block]
[tile]
css img

	[meta]
	#title
	css img[title]

	#src
	css img[src]

[block]
#block1
css .list1

[block]
#block2
css .list2
";

            var m = RuiJiBlockParser.ParserBlock(block);
            var exp = Converter.ToExpression(m);

            Assert.True(m.Metas.Count > 0);
        }

        [Fact]
        public void TestPaging()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("https://www.kuaidaili.com/free/inha/1/");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var block = new ExtractBlock();
            var s = RuiJiBlockParser.ParserBlock(@"
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

            var result = RuiJiExtractor.Extract(content, s);

            Assert.True(true);
        }

        [Fact]
        public void TestExtract()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.ruijihg.com/%e5%bc%80%e5%8f%91/");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var block = new ExtractBlock();
            var s = RuiJiBlockParser.ParserBase("css a[href]").Selectors;
            block.TileSelector.Selectors.AddRange(s);
            var result = RuiJiExtractor.Extract(content, block);

            Assert.True(true);
        }

        [Fact]
        public void TestExtract2()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("https://www.oschina.net/blog");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var parser = new RuiJiParser();
            var eb = parser.ParseExtract("css a.blog-title-link[href]\nexp https://my.oschina.net/*/blog/*");
            var result = RuiJiExtractor.Extract(content, eb.Result);

            Assert.True(true);
        }

        [Fact]
        public void TestExtractTile()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.ruijihg.com/archives/category/tech/bigdata");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var parser = new RuiJiParser();
            var eb = parser.ParseExtract(@"[tile]
css article:html

    [meta]
	#title
	css .entry-header:text

	#summary
	css .entry-header + p:text
	ex /Read more »/ -e");

            var result = RuiJiExtractor.Extract(content, eb.Result);

            Assert.True(true);
        }

        [Fact]
        public void TestExtractMeta()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("https://my.oschina.net/zhupingqi/blog/1826317");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var parser = new RuiJiParser();
            var eb = parser.ParseExtract(@"
[meta]
	#title
	css h1.header:text

	#author
	css div.blog-meta .avatar + span:text

	#date
	css div.blog-meta > div.item:first:text
	regS /发布于/ 1

	#words_i
	css div.blog-meta > div.item:eq(1):text
	regS / / 1

	#content
	css #articleContent:html");

            var result = RuiJiExtractor.Extract(content, eb.Result);

            Assert.True(true);
        }

        [Fact]
        public void TestHistoryUpdate()
        {
            var feed = FeedLiteDb.GetFeed(1);
            var job = new FeedExtractJob();
            job.DoTask(@"D:\云同步\vcoded\RuiJi.Net\RuiJi.Cmd\bin\Debug\snapshot\1_636635303414097356.json");

            Assert.True(true);
        }

        [Fact]
        public void TestUrlExtract()
        {
            var result = Cooperater.GetResult("http://www.cannews.com.cn/2018/0606/177699.shtml");

            Assert.True(true);
        }

        [Fact]
        public void TestCrawlTaskFunc()
        {
            var task = new ParallelTask();
            var model = new CrawlTaskModel();
            model.FeedId = 3;

            var fun = new CrawlTaskFunc();
            var result = fun.Run(model, task);

            Assert.True(result != null);
        }

//        [Fact]
//        public void TestJsonPExtract()
//        {
//            var url = "http://app.cannews.com.cn/roll.php?do=query&callback=jsonp1475197217819&_={# ticks() #}&date={# now(\"yyyy-MM-dd\") #}&size=20&page=1";

//            var f = new JSUrlCompile();
//            //url = f.Compile(url);

//            var c = new RuiJiCrawler();
//            var response = c.Request(new Request(url));

//            var expression = @"
//reg /jsonp[\d]+?\((.*)\)/ 1
//jpath $..url
//";
//            var b = RuiJiBlockParser.ParserBlock(expression);
//            var result = RuiJiExtractor.Extract(response.Data.ToString(), b);

//            Assert.True(result.Content.ToString().Length > 0);
//        }

        [Fact]
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

            var m = RuiJiBlockParser.ParserBlock(block);

            Assert.True(m.Metas.Count > 0);
        }

        [Fact]
        public void TestAdvExpression1()
        {
            var parser = new RuiJiParser();
            parser.ParseFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "expression_address.txt"));

            Assert.True(true);
        }
    }
}
