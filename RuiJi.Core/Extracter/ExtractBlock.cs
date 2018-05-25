using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Core.Extracter.Selector;

namespace RuiJi.Core.Extracter
{
    public class ExtractBlock : ExtractBase
    {
        [JsonProperty("tiles",NullValueHandling = NullValueHandling.Ignore)]
        public ExtractTile TileSelector { get; set; }

        [JsonProperty("blocks", NullValueHandling = NullValueHandling.Ignore)]
        public ExtractBlockCollection Blocks { get; set; }

        [JsonProperty("metas", NullValueHandling = NullValueHandling.Ignore)]
        public ExtractMetaCollection Metas { get; set; }

        public ExtractBlock(string name = "") : base(name)
        {
            Blocks = new ExtractBlockCollection();
            Metas = new ExtractMetaCollection();
            TileSelector = new ExtractTile();
        }
    }
}