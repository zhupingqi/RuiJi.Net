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
    public class ReplaceProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector sel, string html, params object[] args)
        {
            var pr = new ProcessResult();
            var spSelector = sel as ReplaceSelector;
            pr.Matches.Add(Regex.Replace(html, spSelector.Value, spSelector.NewChar));
            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector sel, string html, params object[] args)
        {
            return ProcessNeed(sel, html, args);
        }
    }
}
