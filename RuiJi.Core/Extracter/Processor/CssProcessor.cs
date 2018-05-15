using CsQuery;
using RuiJi.Core.Extracter;
using RuiJi.Core.Extracter.Enum;
using RuiJi.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace RuiJi.Core.Extracter.Processor
{
    public class CssProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector sel, string html, params object[] args)
        {
            var pr = new ProcessResult();

            if (string.IsNullOrEmpty(sel.Value))
            {
                return pr;
            }
            html = "<doc>" + html + "</doc>";
            CssSelector selector = sel as CssSelector;
            if (selector.Type == CssTypeEnum.Text)
                html = ClearTag(html);
            CQ cq = CQ.Create(html, HtmlParsingMode.Auto);

            var elems = cq.Find(selector.Value);

            pr = ProResult(elems, selector);

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector sel, string html, params object[] args)
        {
            CssSelector selector = sel as CssSelector;
            CQ cq = new CQ(html);
            cq[selector.Value].Remove();
            html = HttpUtility.HtmlDecode(cq.Render());
            html = "<doc>" + html + "</doc>";
            cq = CQ.Create(html, HtmlParsingMode.Auto);
            var pr = new ProcessResult();
            pr = ProResult(cq, selector);

            return pr;
        }

        private ProcessResult ProResult(CQ elems, CssSelector selector)
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

        private string ClearTag(string input)
        {
            input = Regex.Replace(input, "<script.*?>.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<style.*?>.*?</style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<iframe.*?>.*?</iframe>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "< type=\"text/javascript\">.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"<div>\s*</div>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<input.*?>", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<input.*?/>", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<textarea.*?>.*?</textarea>", "<textarea></textarea>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<!--.*?-->", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<form.*?>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "</form>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "<font.*?>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "</font>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "<select.*?>.*?</select>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return input.Trim();
        }
    }
}