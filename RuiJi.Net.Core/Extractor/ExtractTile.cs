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
    /// extract tile
    /// </summary>
    public class ExtractTile : ExtractBase
    {
        /// <summary>
        /// tile metas
        /// </summary>
        [JsonProperty("metas", NullValueHandling = NullValueHandling.Ignore)]
        public ExtractMetaCollection Metas { get; set; }

        /// <summary>
        /// page selectors
        /// </summary>
        [JsonProperty("paging", ItemConverterType = typeof(ISelectorConverter))]
        public ExtractBlock Paging { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">tile name</param>
        public ExtractTile(string name = "") : base(name)
        {
            Metas = new ExtractMetaCollection();
        }
    }
}