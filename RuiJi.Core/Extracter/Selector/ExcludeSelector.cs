using Newtonsoft.Json;
using RuiJi.Core.Extracter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter.Selector
{
    public class ExcludeSelector : SelectorBase
    {
        [JsonProperty("type")]
        public ExcludeTypeEnum Type { get; set; }

        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.Exclude;
        }

        public ExcludeSelector()
        { }

        public ExcludeSelector(ExcludeTypeEnum type = ExcludeTypeEnum.BEGIN)
        { }

        public ExcludeSelector(string value,ExcludeTypeEnum type = ExcludeTypeEnum.BEGIN)
        {
            this.Value = value;
            this.Type = type;
        }
    }
}