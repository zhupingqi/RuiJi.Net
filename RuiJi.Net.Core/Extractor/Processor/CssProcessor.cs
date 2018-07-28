using CsQuery;
using RuiJi.Net.Core.Extractor.Enum;
using RuiJi.Net.Core.Extractor.Selector;
using RuiJi.Net.Core.Utils;
using System.Web;

namespace RuiJi.Net.Core.Extractor.Processor
{
    /// <summary>
    /// css processor
    /// </summary>
    public class CssProcessor : ProcessorBase<CssSelector>
    {
        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector">css selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessNeed(CssSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();

            if (string.IsNullOrEmpty(selector.Selector))
            {
                return pr;
            }

            var content = result.Content.Trim();

            if (content.StartsWith("<td"))
            {
                content = "<tr>" + content + "</tr>";
            }

            if(content.StartsWith("<tr"))
            {
            }
            else
            {
                content = "<div>" + content + "</div>";
            }

            if (selector.Type == CssTypeEnum.TEXT)
                content = HtmlHelper.ClearTag(content);

            CQ cq = CQ.Create(content, HtmlParsingMode.Auto,HtmlParsingOptions.AllowSelfClosingTags,DocType.XHTML);
            var elems = cq.Find(selector.Selector);

            pr = ProcessResult(elems, selector);

            return pr;
        }

        /// <summary>
        /// process remove
        /// </summary>
        /// <param name="selector">css selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessRemove(CssSelector selector, ProcessResult result)
        {
            CQ cq = new CQ(result.Content);
            cq[selector.Selector].Remove();
            var content = HttpUtility.HtmlDecode(cq.Render());
            content = "<doc>" + content + "</doc>";
            cq = CQ.Create(content, HtmlParsingMode.Auto);

            var pr = new ProcessResult();
            pr = ProcessResult(cq, selector);

            return pr;
        }

        /// <summary>
        /// match selector result
        /// </summary>
        /// <param name="elems">cq element</param>
        /// <param name="selector">css selector</param>
        /// <returns>process result</returns>
        private ProcessResult ProcessResult(CQ elems, CssSelector selector)
        {
            var pr = new ProcessResult();
            if (elems != null)
            {
                switch (selector.Type)
                {
                    case CssTypeEnum.INNERHTML:
                        {
                            foreach (var ele in elems)
                            {
                                pr.Matches.Add(HttpUtility.HtmlDecode(ele.InnerHTML));
                            }
                            break;
                        }
                    case CssTypeEnum.TEXT:
                        {
                            foreach (var ele in elems)
                            {
                                pr.Matches.Add(HttpUtility.HtmlDecode(ele.Cq().Text()));
                            }
                            break;
                        }
                    case CssTypeEnum.ATTR:
                        {
                            foreach (var ele in elems)
                            {
                                if (!string.IsNullOrEmpty(selector.AttrName))
                                {
                                    var attr = ele.Attributes.GetAttribute(selector.AttrName);
                                    pr.Matches.Add(HttpUtility.HtmlDecode(attr));
                                }
                            }
                            break;
                        }
                    case CssTypeEnum.OUTERHTML:
                        {
                            foreach (var ele in elems)
                            {
                                pr.Matches.Add(HttpUtility.HtmlDecode(ele.OuterHTML));
                            }
                            break;
                        }
                }
            }
            return pr;
        }
    }
}