using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using RuiJi.Net.Core.Extracter.Selector;
using RuiJi.Net.Core.Extracter;

namespace RuiJi.Net.Core.Extracter.Processor
{
    public class TextRangeProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            var textSelector = selector as TextRangeSelector;
            var content = result.Content;

            var b = Regex.Matches(content, textSelector.Begin);
            var e = Regex.Matches(content, textSelector.End, RegexOptions.RightToLeft);

            if (b.Count == 0 || e.Count == 0)
            {
                pr.Matches.Add(content);
                return pr;
            }

            var bt = b[0].Value;
            var et = e[0].Value;

            var begin = content.IndexOf(bt);
            var end = content.LastIndexOf(et);

            content = content.Substring(0, end);
            content = content.Substring(begin + bt.Length);
            pr.Matches.Add(content);

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, ProcessResult result)
        {
            var textSelector = selector as TextRangeSelector;
            var content = result.Content;

            var pr = new ProcessResult();
            var b = Regex.Matches(content, textSelector.Begin);
            var e = Regex.Matches(content, textSelector.End, RegexOptions.RightToLeft);

            if (b.Count == 0 || e.Count == 0)
            {
                pr.Matches.Add(content);
                return pr;
            }
            var bt = b[0].Value;
            var et = e[0].Value;


            var begin = content.IndexOf(bt);
            var end = content.LastIndexOf(et);

            var t = content.Substring(0, end);
            t = t.Substring(begin + bt.Length);
            pr.Matches.Add(content.Replace(t, ""));

            return pr;
        }
    }
}