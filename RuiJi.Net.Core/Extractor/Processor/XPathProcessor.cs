using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using RuiJi.Net.Core.Extractor.Enum;
using RuiJi.Net.Core.Extractor.Selector;

namespace RuiJi.Net.Core.Extractor.Processor
{
    /// <summary>
    /// xpath processor
    /// </summary>
    public class XPathProcessor : ProcessorBase<XPathSelector>
    {
        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector">xml path selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessNeed(XPathSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            if (string.IsNullOrEmpty(selector.XPath))
            {
                return pr;
            }

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(result.Content);

                var nodes = doc.SelectNodes(selector.XPath);
                pr = ProcessResult(nodes, selector);
            }
            catch
            {
            }

            return pr;
        }

        /// <summary>
        /// process remove
        /// </summary>
        /// <param name="selector">xml path selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessRemove(XPathSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            if (string.IsNullOrEmpty(selector.XPath))
            {
                return pr;
            }

            var doc = new XmlDocument();
            doc.LoadXml(result.Content);

            var nodes = doc.SelectNodes(selector.XPath);
            foreach (XmlNode node in nodes)
            {
                doc.RemoveChild(node);
            }

            pr.Matches.Add(doc.OuterXml);

            return pr;
        }

        /// <summary>
        /// prcess xml node
        /// </summary>
        /// <param name="nodes">xml nodes</param>
        /// <param name="selector">xml path selector</param>
        /// <returns>process result</returns>
        private ProcessResult ProcessResult(XmlNodeList nodes, XPathSelector selector)
        {
            var pr = new ProcessResult();
            switch (selector.Type)
            {
                case XPathTypeEnum.INNERXML:
                    {
                        foreach (XmlNode node in nodes)
                        {
                            pr.Matches.Add(node.InnerXml);
                        }
                        break;
                    }
                case XPathTypeEnum.TEXT:
                    {
                        foreach (XmlNode node in nodes)
                        {
                            pr.Matches.Add(node.InnerText);
                        }
                        break;
                    }
                case XPathTypeEnum.ATTR:
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
                case XPathTypeEnum.OUTERXML:
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