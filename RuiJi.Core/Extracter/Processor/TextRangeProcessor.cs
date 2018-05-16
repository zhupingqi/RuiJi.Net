using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using RuiJi.Core.Extracter.Selector;
using RuiJi.Core.Extracter;

namespace RuiJi.Core.Extracter.Processor
{
    public class TextRangeProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector sel, string html, params object[] args)
        {
            var pr = new ProcessResult();
            var spSelector = sel as TextRangeSelector;

            var b = Regex.Matches(html, spSelector.Begin);
            var e = Regex.Matches(html, spSelector.End, RegexOptions.RightToLeft);

            if (b.Count == 0 || e.Count == 0)
            {
                pr.Matches.Add(html);
                return pr;
            }

            var bt = b[0].Value;
            var et = e[0].Value;

            var begin = html.IndexOf(bt);
            var end = html.LastIndexOf(et);

            html = html.Substring(0, end);
            html = html.Substring(begin + bt.Length);
            pr.Matches.Add(html);

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector sel, string html, params object[] args)
        {
            var spSelector = sel as TextRangeSelector;
            var pr = new ProcessResult();
            var b = Regex.Matches(html, spSelector.Begin);
            var e = Regex.Matches(html, spSelector.End, RegexOptions.RightToLeft);

            if (b.Count == 0 || e.Count == 0)
            {
                pr.Matches.Add(html);
                return pr;
            }
            var bt = b[0].Value;
            var et = e[0].Value;


            var begin = html.IndexOf(bt);
            var end = html.LastIndexOf(et);

            var t = html.Substring(0, end);
            t = t.Substring(begin + bt.Length);
            pr.Matches.Add(html.Replace(t, ""));

            return pr;
        }
    }
}