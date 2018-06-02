using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using RuiJi.Core.Extracter.Enum;
using RuiJi.Core.Extracter.Selector;

namespace RuiJi.Core.Extracter.Processor
{
    public class XPathProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            if (string.IsNullOrEmpty(selector.Value))
            {
                return pr;
            }

            try
            {
                var xpathSelector = selector as XPathSelector;

                var doc = new XmlDocument();
                doc.LoadXml(result.Content);

                var nodes = doc.SelectNodes(xpathSelector.Value);
                pr = ProcessResult(nodes, xpathSelector);
            }
            catch { }

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            if (string.IsNullOrEmpty(selector.Value))
            {
                return pr;
            }

            var xpathSelector = selector as XPathSelector;

            var doc = new XmlDocument();
            doc.LoadXml(result.Content);

            var nodes = doc.SelectNodes(xpathSelector.Value);
            foreach (XmlNode node in nodes)
            {
                doc.RemoveChild(node);
            }

            pr.Matches.Add(doc.OuterXml);

            return pr;
        }

        private ProcessResult ProcessResult(XmlNodeList nodes, XPathSelector selector)
        {
            var pr = new ProcessResult();
            switch (selector.Type)
            {
                case XPathTypeEnum.InnerXml:
                    {
                        foreach (XmlNode node in nodes)
                        {
                            pr.Matches.Add(node.InnerXml);
                        }
                        break;
                    }
                case XPathTypeEnum.InnerText:
                    {
                        foreach (XmlNode node in nodes)
                        {
                            pr.Matches.Add(node.InnerText);
                        }
                        break;
                    }
                case XPathTypeEnum.Attr:
                    {
                        foreach (XmlNode node in nodes)
                        {
                            if (!string.IsNullOrEmpty(selector.AttrName))
                            {
                                var attr = node.Attributes[selector.AttrName].Value;
                                pr.Matches.Add(attr);
                            }
                        }
                        break;
                    }
                case XPathTypeEnum.OuterXml:
                    {
                        foreach (XmlNode node in nodes)
                        {
                            pr.Matches.Add(node.OuterXml);
                        }
                        break;
                    }
            }
            return pr;
        }
    }
}