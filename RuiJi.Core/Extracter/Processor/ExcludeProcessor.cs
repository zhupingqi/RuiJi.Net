using RuiJi.Core.Extracter;
using RuiJi.Core.Extracter.Enum;
using RuiJi.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter.Processor
{
    /// <summary>
    /// exclude selector remove selector value
    /// </summary>
    public class ExcludeProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector sel, string html, params object[] args)
        {
            var spSelector = sel as ExcludeSelector;
            var type = spSelector.Type;

            var pr = new ProcessResult();

            switch (type)
            {
                case ExcludeTypeEnum.ALL:
                    pr.Matches.Add(Regex.Replace(html, spSelector.Value, ""));
                    break;
                case ExcludeTypeEnum.BEGIN:
                    pr.Matches.Add(Regex.Replace(html, "^" + spSelector.Value, ""));
                    break;
                case ExcludeTypeEnum.END:
                    pr.Matches.Add(Regex.Replace(html, spSelector.Value + "$", ""));
                    break;

            }

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector sel, string html, params object[] args)
        {
            return ProcessNeed(sel, html, args);
        }
    }
}