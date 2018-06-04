using Newtonsoft.Json;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extracter
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