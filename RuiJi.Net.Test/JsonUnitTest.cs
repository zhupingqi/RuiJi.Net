using Newtonsoft.Json;
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
    }
}
