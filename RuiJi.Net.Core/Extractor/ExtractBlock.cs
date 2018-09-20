using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Net.Core.Expression;
using RuiJi.Net.Core.Extractor.Selector;

namespace RuiJi.Net.Core.Extractor
{
    /// <summary>
    /// extract block
    /// </summary>
    public class ExtractBlock : ExtractBase, IRuiJiParseResult
    {
        /// <summary>
        /// block tiles
        /// </summary>
        [JsonProperty("tile",NullValueHandling = NullValueHandling.Ignore)]
        public ExtractTile TileSelector { get; set; }

        /// <summary>
        /// block sub blocks
        /// </summary>
        [JsonProperty("blocks", NullValueHandling = NullValueHandling.Ignore)]
        public ExtractBlockCollection Blocks { get; set; }

        /// <summary>
        /// block metas
        /// </summary>
        [JsonProperty("metas", NullValueHandling = NullValueHandling.Ignore)]
        public ExtractMetaCollection Metas { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">block name</param>
        public ExtractBlock(string name = "") : base(name)
        {
            Blocks = new ExtractBlockCollection();
            Metas = new ExtractMetaCollection();
            TileSelector = new ExtractTile();
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ExtractBlock():this("")
        {
        }
    }
}