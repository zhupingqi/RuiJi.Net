using CsQuery;
using RuiJi.Core.Extracter.Enum;
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
                    results.Add(key, value[0]);
            }

            return results;
        }

        public static ExtractBlock PaserBlock(string expression)
        {
            expression = expression.Replace("\r\n", "\n").Trim();
            var lines = Split(expression, new string[] { "[blocks]", "[tile]", "[meta]" });
            var block = new ExtractBlock();

            foreach (var key in lines.Keys)
            {
                switch(key)
                {
                    case "":
                        {
                            var b = ParserBase(lines[key]);
                            block.Name = b.Name;
                            block.Selectors = b.Selectors;
                            break;
                        }
                    case "[blocks]":
                        {                            
                            break;
                        }
                    case "[tile]":
                        {
                            block.TileSelector = PaserTile(lines[key]);
                            break;
                        }
                    case "[meta]":
                        {
                            block.Metas = PaserMeta(lines[key]);
                            break;
                        }
                }
            }

            return block;
        }

        public static ExtractTile PaserTile(string expression)
        {
            expression = expression.Trim();
            var lines = Regex.Split(expression, @"\s+\[meta\]\n");

            var tile = new ExtractTile();
            var b = ParserBase(lines.First());
            tile.Name = b.Name;
            tile.Selectors = b.Selectors;
            tile.Metas = PaserMeta(lines.Last());

            return tile;
        }

        public static ExtractMetaCollection PaserMeta(string expression)
        {
            expression = expression.Replace("\r\n", "\n");
            var metas = Regex.Split(expression, @"\n[\s]*\n");

            var mc = new ExtractMetaCollection();

            foreach (var meta in metas)
            {
                var eb = ParserBase(meta);
                if (eb != null)
                    mc.AddMeta(eb);
            }

            return mc;
        }

        public static ExtractBase ParserBase(string expression)
        {
            expression = expression.Trim();
            var lines = expression.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var eb = new ExtractBase();

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("#"))
                {
                    eb.Name = line.Trim().TrimStart('#');
                }
                else
                {
                    var s = ParserSelector(line.Trim());
                    if (s != null)
                        eb.Selectors.Add(s);
                }
            }

            return eb;
        }

        public static ISelector ParserSelector(string expression)
        {
            expression = expression.Trim();
            var sp = expression.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (sp.Length == 0)
                return null;

            var cmd = sp[0];
            var remove = Regex.IsMatch(expression, @".*\-r$");
            var p = Regex.Replace(expression, "^" + cmd, "");
            p = Regex.Replace(p, @"\-r$", "").Trim();

            switch (cmd)
            {
                case "css":
                    {
                        var selector = new CssSelector();
                        selector.Remove = remove;

                        if (p.EndsWith(":ohtml"))
                        {
                            selector.Type = CssTypeEnum.OutHtml;
                            selector.Value = Regex.Replace(p, ":ohtml$", "").Trim();
                        }
                        else if (p.EndsWith(":html"))
                        {
                            selector.Type = CssTypeEnum.InnerHtml;
                            selector.Value = Regex.Replace(p, ":html", "").Trim();
                        }
                        else if (p.EndsWith(":text"))
                        {
                            selector.Type = CssTypeEnum.Text;
                            selector.Value = Regex.Replace(p, ":text", "").Trim();
                        }
                        else
                        {
                            selector.Type = CssTypeEnum.Attr;
                            var ms = Regex.Match(p, @"(.*)\[(.*?)\]");
                            if (ms.Groups.Count == 3)
                            {
                                selector.Value = ms.Groups[1].Value.Trim();
                                selector.AttrName = ms.Groups[2].Value.Trim();
                            }
                            else
                            {
                                return null;
                            }
                        }

                        return selector;
                    }
                case "ex":
                    {
                        var selector = new ExcludeSelector();
                        selector.Remove = remove;
                        selector.Value = Regex.Replace(p, @"\-[bea]?$", "").Trim();
                        if (selector.Value.StartsWith("/") && selector.Value.StartsWith("/"))
                            selector.Value = selector.Value.TrimStart('/').TrimEnd('/');

                        if (p.EndsWith("-b"))
                        {
                            selector.Type = ExcludeTypeEnum.BEGIN;
                        }
                        else if (p.EndsWith("-e"))
                        {
                            selector.Type = ExcludeTypeEnum.END;
                        }
                        else
                        {
                            selector.Type = ExcludeTypeEnum.ALL;
                        }

                        return selector;
                    }
                case "exp":
                    {
                        var selector = new ExpressionSelector();
                        selector.Remove = remove;
                        selector.Value = p;
                        var ssp = p.Split(new string[] { " -s " },StringSplitOptions.RemoveEmptyEntries);
                        if(ssp.Length == 2)
                        {
                            selector.Value = ssp[0];
                            selector.Split = ssp[1];
                        }

                        return selector;
                    }
                case "reg":
                    {
                        var selector = new RegexSelector();
                        selector.Remove = remove;

                        var ms = Regex.Match(p, @"/?(.*)/([\d\s]*)");
                        if (ms.Groups.Count == 2)
                        {
                            selector.Value = ms.Groups[0].Value;
                            return selector;
                        }
                        if (ms.Groups.Count == 3)
                        {
                            selector.Value = ms.Groups[1].Value;
                            selector.Index = ms.Groups[2].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(m => Convert.ToInt32(m)).ToArray();

                            return selector;
                        }
                        return null;
                    }
                case "regR":
                    {
                        var selector = new RegexReplaceSelector();
                        selector.Remove = remove;

                        var ms = Regex.Match(p, @"^/(.*?)/[\s]+(.*)");

                        if (ms.Groups.Count == 2)
                        {
                            selector.Value = ms.Groups[1].Value;
                            selector.NewChar = "";
                            return selector;
                        }
                        if (ms.Groups.Count == 3)
                        {
                            selector.Value = ms.Groups[1].Value;
                            selector.NewChar = ms.Groups[2].Value;

                            return selector;
                        }
                        return null;
                    }
                case "regS":
                    {
                        var selector = new RegexSplitSelector();
                        selector.Remove = remove;

                        var ms = Regex.Match(p, @"/?(.*)/([\d\s]*)");
                        if (ms.Groups.Count == 3)
                        {
                            selector.Value = ms.Groups[1].Value;
                            selector.Index = ms.Groups[2].Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(m => Convert.ToInt32(m)).ToArray();
                            return selector;
                        }

                        return null;
                    }
                case "text":
                    {
                        var selector = new TextRangeSelector();
                        selector.Remove = remove;

                        var ms = Regex.Matches(p, @"/(.*?[^\\\/])/");

                        if (ms.Count == 2)
                        {
                            selector.Begin = ms[0].Value;
                            selector.End = ms[1].Value;

                            return selector;
                        }

                        return null;
                    }
                case "xpath":
                    {
                        var selector = new XPathSelector();
                        selector.Remove = remove;
                        selector.Value = p;

                        return selector;
                    }
                case "jpath":
                    {
                        var selector = new JsonPathSelector();
                        selector.Remove = remove;
                        selector.Value = p;

                        return selector;
                    }
            }

            return null;
        }

        private static Dictionary<string, string> Split(string expression, string[] splits)
        {
            var lines = Regex.Split(expression, @"\n");
            var dic = new Dictionary<string, string>();
            var key = "";
            dic.Add(key, "");

            foreach (var line in lines)
            {
                if(splits.Contains(line))
                {
                    dic.Add(line, "");
                    key = line;
                }
                else
                {
                    if (string.IsNullOrEmpty(dic[key]))
                        dic[key] = line;
                    else
                        dic[key] += "\n" + line;
                }
            }

            return dic;
        }
    }
}