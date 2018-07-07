using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Processor
{
    /// <summary>
    /// regex replace processor
    /// </summary>
    public class RegexReplaceProcessor : ProcessorBase<RegexReplaceSelector>
    {
        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector">regex replace selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessNeed(RegexReplaceSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            pr.Matches.Add(Regex.Replace(result.Content, selector.Pattern, selector.NewString));
            return pr;
        }

        /// <summary>
        /// process remove
        /// </summary>
        /// <param name="selector">regex replace selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessRemove(RegexReplaceSelector selector, ProcessResult result)
        {
            return ProcessNeed(selector, result);
        }
    }
}
