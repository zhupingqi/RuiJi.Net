using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extracter.Processor
{
    public class RegexSplitSelectorProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, ProcessResult result)
        {
            var regSSelector = selector as RegexSplitSelector;
            var sp = Regex.Split(result.Content, regSSelector.Value).ToList();
            sp.RemoveAll(m => string.IsNullOrEmpty(m));

            var results = new List<string>();

            foreach (var index in regSSelector.Index)
            {
                if (index < sp.Count)
                    results.Add(sp[index]);
            }

            var pr = new ProcessResult();
            pr.Matches.Add(string.Join(" ", results.ToArray()));

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, ProcessResult result)
        {
            var regSSelector = selector as RegexSplitSelector;
            var sp = Regex.Split(result.Content, regSSelector.Value, RegexOptions.IgnorePatternWhitespace).ToList();
            sp.RemoveAll(m => string.IsNullOrEmpty(m));

            var results = new List<string>();

            foreach (var index in regSSelector.Index.OrderByDescending(m => m))
            {
                sp.RemoveAt(index);
            }

            var pr = new ProcessResult();
            pr.Matches.Add(string.Join(" ", sp.ToArray()));

            return pr;
        }
    }
}