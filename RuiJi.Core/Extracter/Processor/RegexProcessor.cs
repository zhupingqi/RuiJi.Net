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
        public override ProcessResult ProcessNeed(ISelector sel, string html, params object[] args)
        {
            var spSelector = sel as RegexSelector;
            var regex = new Regex(spSelector.Value);
            var m = regex.Match(html);

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

        public override ProcessResult ProcessRemove(ISelector sel, string html, params object[] args)
        {
            var spSelector = sel as RegexSelector;

            var pr = new ProcessResult();
            pr.Matches.Add(Regex.Replace(html, spSelector.Value, ""));

            return pr;
        }
    }
}