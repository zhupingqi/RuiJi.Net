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
        public override ProcessResult ProcessNeed(ISelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            var expSelector = selector as ExpressionSelector;

            if (string.IsNullOrEmpty(expSelector.Value))
            {
                return pr;
            }

            var ary = new string[] { result.Content };

            if (result == null)
            {
                if (!string.IsNullOrEmpty(expSelector.Split))
                {
                    ary = result.Content.Split(new string[] { expSelector.Split }, StringSplitOptions.RemoveEmptyEntries);
                }

                foreach (var item in ary)
                {
                    var m = Wildcard.IsMatch(item, new string[] { expSelector.Value });
                    if (m)
                        pr.Matches.Add(expSelector.Value);
                }
            }
            else
            {
                foreach (var item in result.Matches)
                {
                    ary = new string[] { item };

                    if (!string.IsNullOrEmpty(expSelector.Split))
                    {
                        ary = item.Split(new string[] { expSelector.Split }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    foreach (var it in ary)
                    {
                        var m = Wildcard.IsMatch(it, new string[] { expSelector.Value });
                        if (m)
                            pr.Matches.Add(it);
                    }
                }
            }

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            var expSelector = selector as ExpressionSelector;

            if (string.IsNullOrEmpty(expSelector.Value))
            {
                return pr;
            }
            var ary = new string[] { result.Content };

            if (result == null)
            {
                if (!string.IsNullOrEmpty(expSelector.Split))
                {
                    ary = result.Content.Split(new string[] { expSelector.Split }, StringSplitOptions.RemoveEmptyEntries);
                }

                foreach (var item in ary)
                {
                    var m = Wildcard.IsMatch(item, new string[] { expSelector.Value });
                    if (!m)
                        pr.Matches.Add(expSelector.Value);
                }
            }
            else
            {
                foreach (var item in result.Matches)
                {
                    ary = new string[] { item };

                    if (!string.IsNullOrEmpty(expSelector.Split))
                    {
                        ary = item.Split(new string[] { expSelector.Split }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    foreach (var it in ary)
                    {
                        var m = Wildcard.IsMatch(it, new string[] { expSelector.Value });
                        if (!m)
                            pr.Matches.Add(it);
                    }
                }
            }

            return pr;
        }
    }
}