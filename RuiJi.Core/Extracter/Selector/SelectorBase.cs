using Newtonsoft.Json;
using RuiJi.Core.Extracter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter.Selector
{
    public abstract class SelectorBase : ISelector
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("remove")]
        public bool Remove { get; set; }

        [JsonProperty("type")]
        public SelectorTypeEnum SelectorType { get; private set; }

        protected abstract SelectorTypeEnum SetSelectType();

        public SelectorBase()
        {
            SelectorType = SetSelectType();
        }
    }
}