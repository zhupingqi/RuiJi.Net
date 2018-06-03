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
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("tiles")]
        public ExtractResultCollection Tiles { get; set; }

        [JsonProperty("blocks")]
        public ExtractResultCollection Blocks { get; set; }

        [JsonProperty("metas")]
        public Dictionary<string, ExtractResult> Metas { get;set; }

        [JsonProperty("paging")]
        public Dictionary<string, string> Paging
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

                var dic = new Dictionary<string, string>();

                foreach (var t in pageBlock.Tiles)
                {
                    if(t.Metas.ContainsKey("page") && t.Metas.ContainsKey("url"))
                        dic.Add(t.Metas["page"].Content, t.Metas["url"].Content);
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