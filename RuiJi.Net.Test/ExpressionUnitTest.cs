using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Core.Crawler;
using RuiJi.Core.Extracter;
using RuiJi.Node.Feed.LTS;

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

            var m = RuiJiExtracter.PaserMeta(metas);

            Assert.IsTrue(m.Count > 0);
        }

        [TestMethod]
        public void TestBlock()
        {
            var block = @"
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
    css h4 a[href] -r";

            block = @"



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
    css h4 a[href] -r";


            var m = RuiJiExtracter.PaserBlock(block);

            Assert.IsTrue(m.Metas.Count > 0);
        }

        [TestMethod]
        public void TestExtract()
        {
            var crawler = new IPCrawler();
            var request = new Request("http://www.ruijihg.com/%e5%bc%80%e5%8f%91/");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var block = new ExtractBlock();
            var s = RuiJiExtracter.ParserBase("css a[href]").Selectors;
            block.TileSelector.Selectors.AddRange(s);
            var result = RuiJiExtracter.Extract(content, block);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestUrlExtract()
        {
            var result = ContentQueue.Instance.Extract("http://www.ruijihg.com/archives/315");           

            Assert.IsTrue(true);
        }
    }
}
