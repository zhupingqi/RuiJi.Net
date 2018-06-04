using CsQuery;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Extracter.Enum;
using RuiJi.Net.Core.Extracter.Selector;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace RuiJi.Net.Core.Extracter.Processor
{
    public class CssProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();

            if (string.IsNullOrEmpty(selector.Value))
            {
                return pr;
            }

            var content = "<doc>" + result.Content + "</doc>";
            var cssSelector = selector as CssSelector;
            if (cssSelector.Type == CssTypeEnum.Text)
                content = HtmlHelper.ClearTag(content);

            CQ cq = CQ.Create(content, HtmlParsingMode.Auto);
            var elems = cq.Find(cssSelector.Value);

            pr = ProcessResult(elems, cssSelector);

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, ProcessResult result)
        {
            var cssSelector = selector as CssSelector;
            CQ cq = new CQ(result.Content);
            cq[cssSelector.Value].Remove();
            var content = HttpUtility.HtmlDecode(cq.Render());
            content = "<doc>" + content + "</doc>";
            cq = CQ.Create(content, HtmlParsingMode.Auto);

            var pr = new ProcessResult();
            pr = ProcessResult(cq, cssSelector);

            return pr;
        }

        private ProcessResult ProcessResult(CQ elems, CssSelector selector)
        {
            var pr = new ProcessResult();
            if (elems != null)
            {
                switch (selector.Type)
                {
                    case CssTypeEnum.InnerHtml:
                        {
                            foreach (var ele in elems)
                            {
                                pr.Matches.Add(HttpUtility.HtmlDecode(ele.InnerHTML));
                            }
                            break;
                        }
                    case CssTypeEnum.Text:
                        {
                            foreach (var ele in elems)
                            {
                                pr.Matches.Add(HttpUtility.HtmlDecode(ele.Cq().Text()));
                            }
                            break;
                        }
                    case CssTypeEnum.Attr:
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
                    case CssTypeEnum.OutHtml:
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