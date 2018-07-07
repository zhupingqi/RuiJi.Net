using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using RuiJi.Net.Core.Extractor.Selector;
using RuiJi.Net.Core.Extractor;

namespace RuiJi.Net.Core.Extractor.Processor
{
    /// <summary>
    /// text range processor
    /// </summary>
    public class TextRangeProcessor : ProcessorBase<TextRangeSelector>
    {
        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector">text range selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessNeed(TextRangeSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            var content = result.Content;

            var b = Regex.Matches(content, selector.Begin);
            var e = Regex.Matches(content, selector.End, RegexOptions.RightToLeft);

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

        /// <summary>
        /// process remove
        /// </summary>
        /// <param name="selector">text range selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessRemove(TextRangeSelector selector, ProcessResult result)
        {
            var content = result.Content;

            var pr = new ProcessResult();
            var b = Regex.Matches(content, selector.Begin);
            var e = Regex.Matches(content, selector.End, RegexOptions.RightToLeft);

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