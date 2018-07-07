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
    /// <summary>
    /// extract base class
    /// </summary>
    public class ExtractBase
    {
        /// <summary>
        /// extract name
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// data type
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// extract selectors
        /// </summary>
        [JsonProperty("selectors",ItemConverterType = typeof(ISelectorConverter))]
        public List<ISelector> Selectors { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">extract name</param>
        public ExtractBase(string name = "")
        {
            Name = name;
            Selectors = new List<ISelector>();
            DataType = typeof(string);
        }
    }
}