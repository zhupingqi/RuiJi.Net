using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Net.Core.Extractor.Selector;
using RuiJi.Net.Core.Utils;

namespace RuiJi.Net.Core.Extractor.Processor
{
    /// <summary>
    /// url expression processor
    /// </summary>
    public class ExpressionProcessor : ProcessorBase<ExpressionSelector>
    {
        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector">expression selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessNeed(ExpressionSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();

            if (string.IsNullOrEmpty(selector.Expression))
            {
                return pr;
            }

            var ary = new string[] { result.Content };

            if (result == null)
            {
                if (!string.IsNullOrEmpty(selector.Split))
                {
                    ary = result.Content.Split(new string[] { selector.Split }, StringSplitOptions.RemoveEmptyEntries);
                }

                foreach (var item in ary)
                {
                    var m = Wildcard.IsMatch(item, new string[] { selector.Expression });
                    if (m)
                        pr.Matches.Add(selector.Expression);
                }
            }
            else
            {
                foreach (var item in result.Matches)
                {
                    ary = new string[] { item };

                    if (!string.IsNullOrEmpty(selector.Split))
                    {
                        ary = item.Split(new string[] { selector.Split }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    foreach (var it in ary)
                    {
                        var m = Wildcard.IsMatch(it, new string[] { selector.Expression });
                        if (m)
                            pr.Matches.Add(it);
                    }
                }
            }

            return pr;
        }

        /// <summary>
        /// process remove
        /// </summary>
        /// <param name="selector">css selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessRemove(ExpressionSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();

            if (string.IsNullOrEmpty(selector.Expression))
            {
                return pr;
            }
            var ary = new string[] { result.Content };

            if (result == null)
            {
                if (!string.IsNullOrEmpty(selector.Split))
                {
                    ary = result.Content.Split(new string[] { selector.Split }, StringSplitOptions.RemoveEmptyEntries);
                }

                foreach (var item in ary)
                {
                    var m = Wildcard.IsMatch(item, new string[] { selector.Expression });
                    if (!m)
                        pr.Matches.Add(selector.Expression);
                }
            }
            else
            {
                foreach (var item in result.Matches)
                {
                    ary = new string[] { item };

                    if (!string.IsNullOrEmpty(selector.Split))
                    {
                        ary = item.Split(new string[] { selector.Split }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    foreach (var it in ary)
                    {
                        var m = Wildcard.IsMatch(it, new string[] { selector.Expression });
                        if (!m)
                            pr.Matches.Add(it);
                    }
                }
            }

            return pr;
        }
    }
}