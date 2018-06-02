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
    public class ExtractTile : ExtractBase
    {
        [JsonProperty("metas", NullValueHandling = NullValueHandling.Ignore)]
        public ExtractMetaCollection Metas { get; set; }

        public ExtractTile(string name = "") : base(name)
        {
            Metas = new ExtractMetaCollection();
        }
    }
}