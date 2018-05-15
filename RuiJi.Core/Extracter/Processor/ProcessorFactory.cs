using RuiJi.Core.Extracter.Enum;
using RuiJi.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter.Processor
{
    public class ProcessorFactory
    {
        private static Dictionary<SelectorTypeEnum, IProcessor> processors;

        static ProcessorFactory()
        {
            processors = new Dictionary<SelectorTypeEnum, IProcessor>();
            processors.Add(SelectorTypeEnum.Css, new CssProcessor());
            processors.Add(SelectorTypeEnum.Regex, new RegexProcessor());
            processors.Add(SelectorTypeEnum.Exclude, new ExcludeProcessor());
            processors.Add(SelectorTypeEnum.RegexSplit, new RegexSelectorProcessor());
            processors.Add(SelectorTypeEnum.Text, new TextProcessor());
            processors.Add(SelectorTypeEnum.Replace, new ReplaceProcessor());
        }

        public static IProcessor GetProcessor(ISelector selector)
        {
            return processors.Keys.Contains(selector.SelectorType) ? processors[selector.SelectorType] : null;
        }
    }
}