using Newtonsoft.Json;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Enum;
using RuiJi.Net.Core.Extractor.Selector;
using System.Collections.Generic;
using Xunit;

namespace RuiJi.Net.Test
{
    public class JsonUnitTest
    {
        [Fact]
        public void TestJson()
        {
            var block = new ExtractBlock();
            block.Selectors = new List<ISelector>
            {
                new CssSelector(".entry-content",CssTypeEnum.INNERHTML)
            };

            block.TileSelector = new ExtractTile
            {
                Selectors = new List<ISelector>
                {
                    new CssSelector(".pt-cv-content-item",CssTypeEnum.INNERHTML)
                }
            };

            block.TileSelector.Metas.AddMeta("title", new List<ISelector> {
                new CssSelector(".pt-cv-title")
            });

            block.TileSelector.Metas.AddMeta("url", new List<ISelector> {
                new CssSelector(".pt-cv-readmore","href")
            });

            var json = JsonConvert.SerializeObject(block);

            var o = JsonConvert.DeserializeObject<ExtractBlock>(json);

            Assert.True(o.Selectors.Count > 0);
        }

        [Fact]
        public void TestConverter()
        {
            var t = new ExtractTile("a");

            var json = JsonConvert.SerializeObject(t);

            Assert.True(true);
        }

        [Fact]
        public void TestJC2()
        {
            var exp = @"
[block]

[blocks]
@block1
@block2

[tile]
css img

	[meta]
	#title
	css img:[title]
	proc aabbcc

	#src
	css img:[src]

	[paging]
	css #listnav a:[href]

[paging]
css #listnav a:[href]

[block]
#block1
css .list1

[block]
#block2
css .list2
";

            var b = RuiJiBlockParser.ParserBlock(exp);

            var json = JsonConvert.SerializeObject(b);

            b = JsonConvert.DeserializeObject<ExtractBlock>(json);

            exp = Converter.ToExpression(b);
            
            Assert.True(b.Metas.Count > 0);

        }
    }
}
