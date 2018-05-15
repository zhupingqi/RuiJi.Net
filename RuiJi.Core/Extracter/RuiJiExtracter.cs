using CsQuery;
using RuiJi.Core.Extracter.Processor;
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

        public List<ExtractResult> Extract(string html, List<ExtractSelector> selectors, bool clear = true, int group = 0)
        {
            var results = new List<ExtractResult>();

            foreach (var selector in selectors)
            {
                var remain = html;
                var pr = new ProcessResult();

                foreach (var sel in selector.Selectors)
                {
                    if (sel == null)
                        continue;

                    var processer = ProcessorFactory.GetProcessor(sel);
                    pr = processer.Process(sel, remain);
                    remain = pr.Html;
                }
                
                if (clear && !string.IsNullOrEmpty(remain))
                {
                    remain = ClearTag(remain);
                    var ccq = CQ.Create(remain, HtmlParsingMode.Auto,HtmlParsingOptions.IgnoreComments);
                    remain = ccq.Render();
                    remain = HttpUtility.HtmlDecode(remain);
                }

                var result = new ExtractResult();
                result.Name = selector.Name;
                result.Value = remain;
                result.Group = group;

                if (selector.HasTile)
                {
                    if (pr.Matches.Count > 1)
                    {
                        result.Tiles = Extract(pr.Matches, selector.TileSelectors, false);
                    }
                    else
                        result.Tiles = Extract(remain, selector.TileSelectors, false);
                }

                results.Add(result);
            }

            return results;
        }

        public List<ExtractResult> Extract(List<string> htmls, List<ExtractSelector> selectors, bool clear = true)
        {
            var results = new List<ExtractResult>();
            var index = 0;

            foreach (var html in htmls)
            {
                var r = Extract(html, selectors, clear, index++);
                results.AddRange(r);
            }

            return results;
        }

        public List<ExtractResult> Extract(string html, ExtractSelecterCollection collection, bool clear = true)
        {
            return Extract(html, collection.Selectors, clear);
        }

        private string ClearTag(string input)
        {
            input = Regex.Replace(input, "<script.*?>.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<style.*?>.*?</style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<iframe.*?>.*?</iframe>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "< type=\"text/javascript\">.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"<div>\s*</div>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<input.*?>", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<input.*?/>", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<textarea.*?>.*?</textarea>", "<textarea></textarea>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<!--.*?-->", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<form.*?>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "</form>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "<font.*?>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "</font>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<select.*?>.*?</select>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return input.Trim();
        }
    }
}