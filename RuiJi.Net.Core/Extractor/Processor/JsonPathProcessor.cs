using RuiJi.Net.Core.Extractor.Selector;
using RuiJi.Net.Core.Extractor.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RuiJi.Net.Core.Extractor.Processor
{
    /// <summary>
    /// json path processor
    /// </summary>
    public class JsonPathProcessor : ProcessorBase<JsonPathSelector>
    {
        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector">jpath selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessNeed(JsonPathSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            if (string.IsNullOrEmpty(selector.JsonPath))
            {
                return pr;
            }

            JObject obj = JObject.Parse(result.Content);
            IEnumerable<JToken> tokens = obj.SelectTokens(selector.JsonPath);

            if (tokens.Count() > 0)
            {
                foreach (JToken t in tokens)
                {
                    pr.Matches.Add(t.ToString());
                }
            }

            return pr;
        }

        /// <summary>
        /// process remove
        /// </summary>
        /// <param name="selector">jpath selector</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public override ProcessResult ProcessRemove(JsonPathSelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            if (string.IsNullOrEmpty(selector.JsonPath))
            {
                return pr;
            }

            JObject obj = JObject.Parse(result.Content);
            JToken token = obj.SelectToken(selector.JsonPath);

            token.Remove();

            pr.Matches.Add(obj.ToString());

            return pr;
        }
    }
}
