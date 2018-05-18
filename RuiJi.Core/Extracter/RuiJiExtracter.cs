using CsQuery;
using RuiJi.Core.Extracter.Processor;
using RuiJi.Core.Extracter.Selector;
using RuiJi.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace RuiJi.Core.Extracter
{
    public class RuiJiExtracter
    {
        public RuiJiExtracter()
        {

        }

        public ExtractResult Extract(string content, ExtractBlock block)
        {
            var pr = ProcessorFactory.Process(content, block.Selectors);

            var result = new ExtractResult
            {
                Name = block.Name,
                Content = pr.Content
            };

            if(block.Blocks!=null && block.Blocks.Count > 0)
            {
                result.Blocks = Extract(result.Content, block.Blocks);
            }

            if (block.TileSelector!= null && block.TileSelector.Selectors.Count > 0)
            {
                result.Tiles = ExtractTile(pr.Content, block.TileSelector);
            }

            if(block.Metas.Count > 0)
            {
                result.Metas = ExtractMeta(pr.Content, block.Metas);
            }

            return result;
        }

        public ExtractResultCollection Extract(string content, ExtractBlockCollection collection)
        {
            var results = new ExtractResultCollection();

            foreach (var block in collection)
            {
                var r = Extract(content, block);
                results.Add(r);
            }

            return results;
        }

        public ExtractResultCollection ExtractSelector(string content, List<ISelector> selectors)
        {
            var pr = ProcessorFactory.Process(content, selectors);

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

        public ExtractResultCollection ExtractTile(string content, ExtractTile tile)
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

                if(tile.Metas.Count > 0)
                {
                    result.Metas = ExtractMeta(m, tile.Metas);
                }

                results.Add(result);
            }

            return results;
        }

        public Dictionary<string, ExtractResult> ExtractMeta(string content, Dictionary<string, List<ISelector>> metas)
        {
            var results = new Dictionary<string, ExtractResult>();

            foreach (var key in metas.Keys)
            {
                var value = ExtractSelector(content, metas[key]);
                if(value.Count > 0)
                    results.Add(key, value[0]);
            }

            return results;
        }
    }
}