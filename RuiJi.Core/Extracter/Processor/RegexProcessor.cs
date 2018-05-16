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
    public class RegexProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, string content, params object[] args)
        {
            var spSelector = selector as RegexSelector;
            var regex = new Regex(spSelector.Value);
            var m = regex.Match(content);

            var results = new List<string>();

            foreach (var index in spSelector.Index)
            {
                if (index < m.Groups.Count)
                    results.Add(m.Groups[index].Value);
            }

            var pr = new ProcessResult();
            pr.Matches.Add(string.Join(" ", results.ToArray()));

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, string content, params object[] args)
        {
            var spSelector = selector as RegexSelector;

            var pr = new ProcessResult();
            pr.Matches.Add(Regex.Replace(content, spSelector.Value, ""));

            return pr;
        }
    }
}