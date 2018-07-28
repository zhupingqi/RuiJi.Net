using Newtonsoft.Json;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Enum;
using RuiJi.Net.Core.Extractor.Selector;
using RuiJi.Net.NodeVisitor;
using RuiJi.Net.Owin;
using RuiJi.Net.Storage;
using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace RuiJi.Net.Test
{
    public class ExtractUnitTest
    {
        [Fact]
        public void TestLocalExtract()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("http://www.ruijihg.com/%e5%bc%80%e5%8f%91/");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

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

            //block.TileSelector.Metas.AddMeta(new ExtractBase {
            //    Name = "title",
            //    Selectors = new List<ISelector> {
            //        new CssSelector(".pt-cv-title")
            //    }
            //});

            //block.TileSelector.Metas.AddMeta(new ExtractBase
            //{
            //    Name = "url",
            //    Selectors = new List<ISelector> {
            //       new CssSelector(".pt-cv-readmore","href")
            //    }
            //});

            block.TileSelector.Metas.AddMeta("title", new List<ISelector> {
                new CssSelector(".pt-cv-title")
            });

            block.TileSelector.Metas.AddMeta("url", new List<ISelector> {
                new CssSelector(".pt-cv-readmore","href")
            });

            var r = RuiJiExtractor.Extract(content, block);

            Assert.True(r.Content.ToString().Length > 0);
            Assert.True(r.Tiles.Count > 0);
        }

        [Fact]
        public void TestNodeExtract()
        {
            ServerManager.StartServers();

            var response = Crawler.Request("http://www.ruijihg.com/%e5%bc%80%e5%8f%91/");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return;

            var content = response.Data.ToString();

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

            var r = Extractor.Extract(new ExtractRequest
            {
                Blocks = new List<ExtractFeatureBlock> {
                    new ExtractFeatureBlock
                    {
                        Block = block
                    }
                },
                Content = content
            });

            Assert.True(r[0].Content.ToString().Length > 0);
            Assert.True(r[0].Tiles.Count > 0);
        }

        [Fact]
        public void TestGetRule()
        {
            ServerManager.StartServers();

            var response = Crawler.Request("http://www.ruijihg.com/2018/05/20/ruiji-solr-net/");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return;

            var content = response.Data.ToString();

            var block = Feeder.GetExtractBlock("http://www.ruijihg.com/2018/05/20/ruiji-solr-net/").First();

            var r = Extractor.Extract(new ExtractRequest
            {
                Blocks = new List<ExtractFeatureBlock> {
                    block
                },
                Content = content
            });

            Assert.True(r[0].Content.ToString().Length > 0);
            Assert.True(r[0].Tiles.Count > 0);
        }

        [Fact]
        public void TestPaging()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("https://www.kuaidaili.com/free/inha/10");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var exp = @"
[tile]
	css table.table-bordered tr:gt(0):ohtml

	[meta]
	#ip
	css td[data-title='IP']:text

    #port
    css td[data-title='PORT']:text

[paging]
css #listnav a[href]";

            var block = RuiJiBlockParser.ParserBlock(exp);
            var result = RuiJiExtractor.Extract(content, block);

            if (result.Paging != null && result.Paging.Count > 0 && result.Tiles != null)
            {
                var storage = new FileStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"www","download"));

                PagingExtractor.CrawlPage(request.Uri, result, block,(u,res)=> {
                    var c = new DownloadContentModel();
                    c.Url = u.AbsolutePath.Trim();
                    c.IsRaw = false;
                    c.Data = JsonConvert.SerializeObject(res.Tiles);

                    storage.Insert(c);
                },int.MaxValue);
            }

            Assert.True(true);
        }

        [Fact]
        public void TestPaging2()
        {
            var crawler = new RuiJiCrawler();
            var request = new Request("https://3w.huanqiu.com/a/4e2d56fd7f51/7DHitRASkPC?p=1&agt=8");

            var response = crawler.Request(request);
            var content = response.Data.ToString();

            var exp = @"
[meta]
	#title
	css h1.a-title

	#date_dt
	css .time:text

	#content
	css .a-con:ohtml

[paging]
css .a-page
css a[href]";

            var block = RuiJiBlockParser.ParserBlock(exp);
            var result = RuiJiExtractor.Extract(content, block);

            if (result.Paging != null && result.Paging.Count > 0 && result.Metas != null && result.Metas.ContainsKey("content"))
            {
                result = PagingExtractor.MergeContent(request.Uri, result, block);
            }

            Assert.True(true);
        }
    }
}