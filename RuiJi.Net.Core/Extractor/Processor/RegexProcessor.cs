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
    public class RegexProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, ProcessResult result)
        {
            var regSelector = selector as RegexSelector;
            var regex = new Regex(regSelector.Value);
            var m = regex.Match(result.Content);

            var results = new List<string>();

            foreach (var index in regSelector.Index)
            {
                if (index < m.Groups.Count)
                    results.Add(m.Groups[index].Value);
            }

            var pr = new ProcessResult();
            pr.Matches.Add(string.Join(" ", results.ToArray()));

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, ProcessResult result)
        {
            var regSelector = selector as RegexSelector;

            var pr = new ProcessResult();
            pr.Matches.Add(Regex.Replace(result.Content, regSelector.Value, ""));

            return pr;
        }
    }
}