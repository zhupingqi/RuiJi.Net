using RuiJi.Core.Extracter;
using RuiJi.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter.Processor
{
    public class RegexSelectorProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, string html, params object[] args)
        {
            var spSelector = selector as RegexSplitSelector;
            var sp = Regex.Split(html, spSelector.Value).ToList();
            sp.RemoveAll(m=>string.IsNullOrEmpty(m));

            var results = new List<string>();

            foreach (var index in spSelector.Index)
            {
                if (index < sp.Count)
                    results.Add(sp[index]);
            }

            var pr = new ProcessResult();
            pr.Matches.Add(string.Join(" ", results.ToArray()));

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, string html, params object[] args)
        {
            var spSelector = selector as RegexSplitSelector;
            var sp = Regex.Split(html, spSelector.Value, RegexOptions.IgnorePatternWhitespace).ToList();
            sp.RemoveAll(m => string.IsNullOrEmpty(m));

            var results = new List<string>();

            foreach (var index in spSelector.Index.OrderByDescending(m=>m))
            {
                sp.RemoveAt(index);
            }

            var pr = new ProcessResult();
            pr.Matches.Add(string.Join(" ", sp.ToArray()));

            return pr;
        }
    }
}