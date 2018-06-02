using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Extracter.Enum;
using RuiJi.Net.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class JsonUnitTest
    {
        [TestMethod]
        public void TestJson()
        {
            var block = new ExtractBlock();
            block.Selectors = new List<ISelector>
            {
                new CssSelector(".entry-content",CssTypeEnum.InnerHtml)
            };

            block.TileSelector = new ExtractTile
            {
                Selectors = new List<ISelector>
                {
                    new CssSelector(".pt-cv-content-item",CssTypeEnum.InnerHtml)
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

            Assert.IsTrue(o.Selectors.Count > 0);
        }

        [TestMethod]
        public void TestConverter()
        {
            var t = new ExtractTile("a");

            var json = JsonConvert.SerializeObject(t);

            Assert.IsTrue(true);
        }
    }
}
