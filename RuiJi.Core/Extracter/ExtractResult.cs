using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter
{
    public class ExtractResult
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("tiles")]
        public ExtractResultCollection Tiles { get; set; }

        [JsonProperty("blocks")]
        public ExtractResultCollection Blocks { get; set; }

        [JsonProperty("metas")]
        public Dictionary<string, ExtractResult> Metas { get; set; }

        public ExtractResult()
        {
            //Tiles = new ExtractResultCollection();
            //Blocks = new ExtractResultCollection();
            //Metas = new  Dictionary<string, ExtractResult>();
        }
    }
}