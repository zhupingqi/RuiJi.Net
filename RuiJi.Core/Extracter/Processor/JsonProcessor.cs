using RuiJi.Core.Extracter.Selector;
using RuiJi.Core.Extracter.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter.Processor
{
    class JsonProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector sel, string html, params object[] args)
        {
            //JObject obj = JObject.Parse(content);
            //JToken token = obj.SelectToken(property);

            //if (token != null)
            //{
            //    return JsonConvert.DeserializeObject<T>(token.ToString());
            //}

            throw new NotImplementedException();
        }

        public override ProcessResult ProcessRemove(ISelector sel, string html, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
