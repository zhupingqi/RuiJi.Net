using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Processor
{
    /// <summary>
    /// regex split processor
    /// </summary>
    public class RegexSplitSelectorProcessor : ProcessorBase<RegexSplitSelector>
    {
        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector">regex split selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessNeed(RegexSplitSelector selector, ProcessResult result)
        {
            var sp = Regex.Split(result.Content, selector.Pattern).ToList();
            sp.RemoveAll(m => string.IsNullOrEmpty(m));

            var results = new List<string>();

            foreach (var index in selector.Index)
            {
                if (index < sp.Count)
                    results.Add(sp[index]);
            }

            var pr = new ProcessResult();
            pr.Matches.Add(string.Join(" ", results.ToArray()));

            return pr;
        }

        /// <summary>
        /// process remove
        /// </summary>
        /// <param name="selector">regex split selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessRemove(RegexSplitSelector selector, ProcessResult result)
        {
            var sp = Regex.Split(result.Content, selector.Pattern, RegexOptions.IgnorePatternWhitespace).ToList();
            sp.RemoveAll(m => string.IsNullOrEmpty(m));

            foreach (var index in selector.Index.OrderByDescending(m => m))
            {
                sp.RemoveAt(index);
            }

            var pr = new ProcessResult();
            pr.Matches.Add(string.Join(" ", sp.ToArray()));

            return pr;
        }
    }
}