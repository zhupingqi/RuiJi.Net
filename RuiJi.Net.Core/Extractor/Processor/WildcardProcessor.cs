using RuiJi.Net.Core.Extractor.Selector;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Processor
{
    public class WildcardProcessor : ProcessorBase<WildcardSelector>
    {
        public override ProcessResult ProcessNeed(WildcardSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();

            foreach (var r in result.Matches)
            {
                if (Wildcard.IsMatch(r, new string[] { selector.Pattern }))
                    pr.Matches.Add(r);
            }

            return pr;
        }

        public override ProcessResult ProcessRemove(WildcardSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();

            foreach (var r in result.Matches)
            {
                if (!Wildcard.IsMatch(r, new string[] { selector.Pattern }))
                    pr.Matches.Add(r);
            }

            return pr;
        }
    }
}
