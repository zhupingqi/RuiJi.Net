using RuiJi.Net.Core.Extracter.Enum;
using RuiJi.Net.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extracter
{
    public class RuiJiExpression
    {
        public static ExtractBlock ParserBlock(string expression)
        {
            expression = expression.Replace("\r\n", "\n").Trim();

            var blockExps = ParserBlocks(expression);
            var results = new List<ExtractBlock>();

            foreach (var exp in blockExps)
            {
                var block = new ExtractBlock();
                var blockExp = exp.Replace("\r\n", "\n").Trim();
                var lines = Split(blockExp, new string[] { "[block]", "[blocks]", "[tile]", "[meta]", "[paging]" });

                foreach (var key in lines.Keys)
                {
                    switch (key)
                    {
                        case "":
                        case "[block]":
                            {
                                var b = ParserBase(lines[key]);
                                block.Name = b.Name;
                                block.Selectors = b.Selectors;
                                break;
                            }
                        case "[blocks]":
                            {
                                var bs = lines[key].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var b in bs)
                                {
                                    if(b.Trim().StartsWith("@"))
                                    {
                                        block.Blocks.Add(new ExtractBlock(b.Trim().TrimStart('@')));
                                    }
                                }
                                break;
                            }
                        case "[tile]":
                            {
                                block.TileSelector = ParserTile(lines[key]);
                                break;
                            }
                        case "[meta]":
                            {
                                block.Metas = ParserMeta(lines[key]);
                                break;
                            }
                        case "[paging]":
                            {
                                var b = new ExtractBlock("paging");
                                string.IsNullOrEmpty(b.Name);
                                b.Name = "paging";
                                b.TileSelector = ParserTile(lines[key]);
                                block.Blocks.Add(b);
                                break;
                            }
                    }
                }

                results.Add(block);
            }

            var removes = new List<ExtractBlock>();

            foreach (var result in results)
            {                
                for (int j = 0; j < result.Blocks.Count; j++)
                {
                    var sub = results.Where(m=> m != null ).SingleOrDefault(m => m.Name == result.Blocks[j].Name);

                    if (sub != null)
                    {
                        result.Blocks[j] = sub;
                        removes.Add(sub);
                    }
                }
            }

            results.RemoveAll(m=> removes.Contains(m));

            return results.First();
        }

        public static ExtractTile ParserTile(string expression)
        {
            expression = expression.Trim();
            var lines = Regex.Split(expression, @"\s+\[meta\]\n");

            var tile = new ExtractTile();
            var b = ParserBase(lines.First());
            tile.Name = b.Name;
            tile.Selectors = b.Selectors;
            tile.Metas = ParserMeta(lines.Last());

            return tile;
        }

        public static ExtractMetaCollection ParserMeta(string expression)
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
                                selector.Type = CssTypeEnum.OutHtml;
                                selector.Value = Regex.Replace(p, ":ohtml$", "").Trim();
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
                        var ssp = p.Split(new string[] { " -s " }, StringSplitOptions.RemoveEmptyEntries);
                        if (ssp.Length == 2)
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

        private static List<string> ParserBlocks(string expression)
        {
            expression = "\n" + expression;

            var results = Regex.Split(expression, @"\n\[block\]").ToList();
            results.RemoveAll(m=>string.IsNullOrEmpty(m));

            return results;
        }

        private static Dictionary<string, string> Split(string expression, string[] splits)
        {
            var lines = Regex.Split(expression, @"\n");
            var dic = new Dictionary<string, string>();
            var key = "";
            dic.Add(key, "");

            foreach (var line in lines)
            {
                if (splits.Contains(line))
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
