using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Core.Extracter.Selector;
using RuiJi.Core.Utils;

namespace RuiJi.Core.Extracter.Processor
{
    public class ExpressionProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector sel, string content, params object[] args)
        {
            var pr = new ProcessResult();
            var selector = sel as ExpressionSelector;

            if (string.IsNullOrEmpty(selector.Value))
            {
                return pr;
            }

            var ary = new string[] { content };

            if (!string.IsNullOrEmpty(selector.Split))
            {
                ary = content.Split(new string[] { selector.Split }, StringSplitOptions.RemoveEmptyEntries);
            }

            foreach (var item in ary)
            {
                var m = Wildcard.IsMatch(item, new string[] { selector.Value });
                if (m)
                    pr.Matches.Add(selector.Value);
            }

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector sel, string content, params object[] args)
        {
            var pr = new ProcessResult();
            var selector = sel as ExpressionSelector;

            if (string.IsNullOrEmpty(selector.Value))
            {
                return pr;
            }

            var ary = new string[] { content };

            if (!string.IsNullOrEmpty(selector.Split))
            {
                ary = content.Split(new string[] { selector.Split }, StringSplitOptions.RemoveEmptyEntries);
            }

            foreach (var item in ary)
            {
                var m = Wildcard.IsMatch(item, new string[] { selector.Value });
                if (!m)
                    pr.Matches.Add(selector.Value);
            }

            return pr;
        }
    }
}