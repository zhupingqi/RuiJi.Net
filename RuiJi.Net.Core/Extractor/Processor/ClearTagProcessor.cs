using RuiJi.Net.Core.Extractor.Selector;
using RuiJi.Net.Core.Utils;

namespace RuiJi.Net.Core.Extractor.Processor
{
    /// <summary>
    /// clear tag processor ,auto clear script,style,iframe,textarea,form,select tag
    /// </summary>
    public class ClearTagProcessor : ProcessorBase<ClearTagSelector>
    {
        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override ProcessResult ProcessNeed(ClearTagSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            pr.Matches.Add(HtmlHelper.ClearTag(result.Content));

            return pr;
        }

        /// <summary>
        /// process remove
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override ProcessResult ProcessRemove(ClearTagSelector selector, ProcessResult result)
        {
            return ProcessNeed(selector, result);
        }
    }
}
