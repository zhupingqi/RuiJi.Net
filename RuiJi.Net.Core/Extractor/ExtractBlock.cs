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
    public class ExtractBlock : ExtractBase, IRuiJiParseResult
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

        public ExtractBlock():this("")
        {

        }
    }
}