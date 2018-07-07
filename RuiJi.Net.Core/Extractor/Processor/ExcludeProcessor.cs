using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Enum;
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
    /// exclude processor, remove selector value
    /// </summary>
    public class ExcludeProcessor : ProcessorBase<ExcludeSelector>
    {
        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector">exclude selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessNeed(ExcludeSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();

            switch (selector.Type)
            {
                case ExcludeTypeEnum.ALL:
                    pr.Matches.Add(Regex.Replace(result.Content, selector.Pattern, ""));
                    break;
                case ExcludeTypeEnum.BEGIN:
                    pr.Matches.Add(Regex.Replace(result.Content, "^" + selector.Pattern, ""));
                    break;
                case ExcludeTypeEnum.END:
                    pr.Matches.Add(Regex.Replace(result.Content, selector.Pattern + "$", ""));
                    break;
            }

            return pr;
        }

        /// <summary>
        /// process remove ,same as process need
        /// </summary>
        /// <param name="selector">exclude selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessRemove(ExcludeSelector selector, ProcessResult result)
        {
            return ProcessNeed(selector, result);
        }
    }
}