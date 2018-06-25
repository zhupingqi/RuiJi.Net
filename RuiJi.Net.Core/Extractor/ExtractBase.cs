using Newtonsoft.Json;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extractor.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor
{
    public enum ContentTypeEnum
    {

    }

    public class ExtractBase
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        public Type ContentType { get; set; }

        [JsonProperty("selectors",ItemConverterType = typeof(ISelectorConverter))]
        public List<ISelector> Selectors { get; set; }

        public ExtractBase(string name = "")
        {
            Name = name;
            Selectors = new List<ISelector>();
            ContentType = typeof(string);
        }
    }
}