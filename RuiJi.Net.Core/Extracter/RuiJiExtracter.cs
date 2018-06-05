using CsQuery;
using RuiJi.Net.Core.Extracter.Enum;
using RuiJi.Net.Core.Extracter.Processor;
using RuiJi.Net.Core.Extracter.Selector;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace RuiJi.Net.Core.Extracter
{
    public class RuiJiExtracter
    {
        public RuiJiExtracter()
        {

        }

        public static ExtractResult Extract(string content, ExtractBlock block)
        {
            var pr = ProcessorFactory.Process(content, block.Selectors);

            var result = new ExtractResult
            {
                Name = block.Name,
                Content = pr.Content
            };

            if (block.Blocks != null && block.Blocks.Count > 0)
            {
                result.Blocks = Extract(result.Content, block.Blocks);
            }

            if (block.TileSelector != null && block.TileSelector.Selectors.Count > 0)
            {
                result.Tiles = ExtractTile(pr.Content, block.TileSelector);
            }

            if (block.Metas.Count > 0)
            {
                result.Metas = ExtractMeta(pr.Content, block.Metas);
            }

            return result;
        }

        public static ExtractResultCollection Extract(string content, ExtractBlockCollection collection)
        {
            var results = new ExtractResultCollection();

            foreach (var block in collection)
            {
                var r = Extract(content, block);
                results.Add(r);
            }

            return results;
        }

        public static ExtractResultCollection ExtractSelector(string content, ExtractBase extractBase)
        {
            var pr = ProcessorFactory.Process(content, extractBase.Selectors);

            var results = new ExtractResultCollection();

            foreach (var m in pr.Matches)
            {
                var result = new ExtractResult
                {
                    Name = "tile",
                    Content = m
                };

                results.Add(result);
            }

            return results;
        }

        public static ExtractResultCollection ExtractTile(string content, ExtractTile tile)
        {
            var pr = ProcessorFactory.Process(content, tile.Selectors);

            var results = new ExtractResultCollection();

            foreach (var m in pr.Matches)
            {
                var result = new ExtractResult
                {
                    Name = "tile",
                    Content = m
                };

                if (tile.Metas.Count > 0)
                {
                    result.Metas = ExtractMeta(m, tile.Metas);
                }

                results.Add(result);
            }

            return results;
        }

        public static Dictionary<string, ExtractResult> ExtractMeta(string content, ExtractMetaCollection metas)
        {
            var results = new Dictionary<string, ExtractResult>();

            foreach (var key in metas.Keys)
            {
                var value = ExtractSelector(content, metas[key]);

                if (value.Count > 0)
                {
                    value[0].Name = key;
                    results.Add(key, value[0]);
                }
            }

            return results;
        }
    }
}