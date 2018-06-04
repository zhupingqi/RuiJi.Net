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
    public class RegexReplaceProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            var regRSelector = selector as RegexReplaceSelector;
            pr.Matches.Add(Regex.Replace(result.Content, regRSelector.Value, regRSelector.NewChar));
            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, ProcessResult result)
        {
            return ProcessNeed(selector, result);
        }
    }
}
