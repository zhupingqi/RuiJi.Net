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
        public override ProcessResult ProcessNeed(ISelector sel, string html, params object[] args)
        {
            var spSelector = sel as ClearTagSelector;

            var pr = new ProcessResult();
            pr.Matches.Add(HtmlHelper.ClearTag(html));

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector sel, string html, params object[] args)
        {
            return ProcessNeed(sel, html, args);
        }
    }
}
