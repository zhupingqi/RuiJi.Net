using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extracter
{
    public class ExtractResult
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public object Content { get; set; }

        [JsonProperty("tiles", NullValueHandling = NullValueHandling.Ignore)]
        public ExtractResultCollection Tiles { get; set; }

        [JsonProperty("blocks", NullValueHandling = NullValueHandling.Ignore)]
        public ExtractResultCollection Blocks { get; set; }

        [JsonProperty("metas", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metas { get;set; }

        [JsonProperty("paging", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Paging
        {
            get
            {
                if (Blocks == null)
                    return null;

                var pageBlock = Blocks.SingleOrDefault(m => m.Name == "paging");

                if (pageBlock == null)
                {
                    return null;
                }

                var dic = new List<string>();

                foreach (var t in pageBlock.Tiles)
                {
                    if(t.Content!= null && !string.IsNullOrEmpty(t.Content.ToString()))
                        dic.Add(t.Content.ToString());
                }

                return dic;
            }
        }

        public ExtractResult()
        {
            //Tiles = new ExtractResultCollection();
            //Blocks = new ExtractResultCollection();
            //Metas = new  Dictionary<string, ExtractResult>();
        }
    }
}