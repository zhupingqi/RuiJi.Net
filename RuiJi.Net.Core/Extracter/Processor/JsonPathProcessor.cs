using RuiJi.Net.Core.Extracter.Selector;
using RuiJi.Net.Core.Extracter.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RuiJi.Net.Core.Extracter.Processor
{
    class JsonPathProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            if (string.IsNullOrEmpty(selector.Value))
            {
                return pr;
            }

            JObject obj = JObject.Parse(result.Content);
            JToken token = obj.SelectToken(selector.Value);

            if (token != null)
            {
                foreach (JToken t in token.Children())
                {
                    pr.Matches.Add(t.ToString());
                }
            }

            return pr;
        }

        public override ProcessResult ProcessRemove(ISelector selector, ProcessResult result)
        {
            var pr = new ProcessResult();
            if (string.IsNullOrEmpty(selector.Value))
            {
                return pr;
            }

            JObject obj = JObject.Parse(result.Content);
            JToken token = obj.SelectToken(selector.Value);

            token.Remove();

            pr.Matches.Add(obj.ToString());

            return pr;
        }
    }
}
