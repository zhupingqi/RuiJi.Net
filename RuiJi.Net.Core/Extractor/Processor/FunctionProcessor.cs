using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RuiJi.Net.Core.Compile;
using RuiJi.Net.Core.Extractor.Selector;

namespace RuiJi.Net.Core.Extractor.Processor
{
    public class FunctionProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, ProcessResult result)
        {
            var funcSelector = selector as FunctionSelector;

            var pr = new ProcessResult();

            var compile = new ProcessorCompile();
            var r = compile.GetResult(new KeyValuePair<string, string>(funcSelector.Value, result.Content));

            if (r.Length > 0)
                pr.Matches.Add(r.First().ToString());
            else
                pr.Matches.Add(result.Content);

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, ProcessResult result)
        {
            //var content = "{0}";

            //if (content.EndsWith("小时前"))
            //{
            //    var hour = Convert.ToInt32(Regex.Match(content, @"[\d]*").Value);
            //    results.Add(DateTime.Now.AddHours(-hour));
            //}

            return ProcessNeed(selector, result);
        }
    }
}