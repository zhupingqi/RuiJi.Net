using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Core.Extracter.Selector;
using RuiJi.Core.Utils;

namespace RuiJi.Core.Extracter.Processor
{
    public class ClearTagProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, ProcessResult result)
        {
            var clearSelector = selector as ClearTagSelector;

            var pr = new ProcessResult();
            pr.Matches.Add(HtmlHelper.ClearTag(result.Content));

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, ProcessResult result)
        {
            return ProcessNeed(selector, result);
        }
    }
}
