using Newtonsoft.Json;
using RuiJi.Core.Extensions;
using RuiJi.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter
{
    public class ExtractBase
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("selectors",ItemConverterType = typeof(ISelectorConverter))]
        public List<ISelector> Selectors { get; set; }

        public ExtractBase(string name = "")
        {
            Name = name;
            Selectors = new List<ISelector>();
        }
    }
}