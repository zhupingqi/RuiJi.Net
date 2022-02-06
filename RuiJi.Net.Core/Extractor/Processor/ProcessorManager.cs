using RuiJi.Net.Core.Extractor.Enum;
using RuiJi.Net.Core.Extractor.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Processor
{
    /// <summary>
    /// processor manager
    /// </summary>
    public class ProcessorManager
    {
        private static Dictionary<SelectorTypeEnum, IProcessor> processors;

        static ProcessorManager()
        {
            processors = new Dictionary<SelectorTypeEnum, IProcessor>();
            processors.Add(SelectorTypeEnum.CSS, new CssProcessor());
            processors.Add(SelectorTypeEnum.REGEX, new RegexProcessor());
            processors.Add(SelectorTypeEnum.EXCLUDE, new ExcludeProcessor());
            processors.Add(SelectorTypeEnum.REGEXSPLIT, new RegexSplitSelectorProcessor());
            processors.Add(SelectorTypeEnum.TEXTRANGE, new TextRangeProcessor());
            processors.Add(SelectorTypeEnum.REGEXREPLACE, new RegexReplaceProcessor());
            processors.Add(SelectorTypeEnum.EXPRESSION, new ExpressionProcessor());
            processors.Add(SelectorTypeEnum.XPATH, new XPathProcessor());
            processors.Add(SelectorTypeEnum.JPATH, new JsonPathProcessor());
            processors.Add(SelectorTypeEnum.CLEAR, new ClearTagProcessor());
            processors.Add(SelectorTypeEnum.FUNCTION, new FunctionProcessor());
            processors.Add(SelectorTypeEnum.WILDCARD, new WildcardProcessor());
        }

        /// <summary>
        /// process content width selectors
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="selectors">selectors</param>
        /// <returns></returns>
        public static ProcessResult Process(string content, List<ISelector> selectors)
        {
            var result = new ProcessResult();
            result.Matches.Add(content);

            if (selectors.Count == 0)
            {
                result.Matches = new List<string>() { content };
            }
            else
            {
                foreach (var selector in selectors)
                {
                    if (selector == null)
                        continue;

                    var processer = processors[selector.SelectorType];
                    result = processer.Process(selector, result);
                }
            }

            return result;
        }
    }
}