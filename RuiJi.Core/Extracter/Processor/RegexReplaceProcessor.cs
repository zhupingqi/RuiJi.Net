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
