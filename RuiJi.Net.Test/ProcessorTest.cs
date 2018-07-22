using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Processor;
using RuiJi.Net.Core.Extractor.Selector;
using Xunit;

namespace RuiJi.Net.Test
{

    public class ProcessorTest
    {
        [Fact]
        public void TestRegexReplace()
        {
            var p = new RegexReplaceProcessor();
            var s = new RegexReplaceSelector();
            s.NewString = ">";
            s.Pattern = ">>";

            var pr = new ProcessResult();
            pr.Matches.Add("评论频道>>民声");

            pr = p.ProcessNeed(s,pr);

            Assert.True(pr.Content.IndexOf(">>") == -1);
        }

        [Fact]
        public void TestCss()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.legaldaily.com.cn/index_article/content/2016-08/17/content_6765457.htm?node=5955");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var p = new CssProcessor();
            var s = new CssSelector();
            s.Selector = "div.f12:first";
            s.Type = Core.Extractor.Enum.CssTypeEnum.TEXT;

            var pr = new ProcessResult();
            pr.Matches.Add(content);

            pr = p.ProcessNeed(s, pr);

            Assert.True(pr.Content.IndexOf(">>") == -1);
        }
    }
}
