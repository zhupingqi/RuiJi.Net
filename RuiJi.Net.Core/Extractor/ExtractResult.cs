using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor
{
    /// <summary>
    /// extract result
    /// </summary>
    public class ExtractResult
    {
        /// <summary>
        /// extract name
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// extract content result
        /// </summary>
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public object Content { get; set; }

        /// <summary>
        /// extract tile result
        /// </summary>
        [JsonProperty("tiles", NullValueHandling = NullValueHandling.Ignore)]
        public ExtractResultCollection Tiles { get; set; }

        /// <summary>
        /// extract sub blocks result
        /// </summary>
        [JsonProperty("blocks", NullValueHandling = NullValueHandling.Ignore)]
        public ExtractResultCollection Blocks { get; set; }

        /// <summary>
        /// extract meta result
        /// </summary>
        [JsonProperty("metas", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metas { get; set; }

        /// <summary>
        /// extract paging result
        /// </summary>
        [JsonProperty("_paging", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Paging
        {
            get
            {
                if (Blocks == null)
                    return null;

                var pageBlock = Blocks.SingleOrDefault(m => m.Name == "_paging");

                if (pageBlock == null)
                {
                    return null;
                }

                var dic = new List<string>();

                foreach (var t in pageBlock.Tiles)
                {
                    if (t.Content != null && !string.IsNullOrEmpty(t.Content.ToString()))
                        dic.Add(t.Content.ToString());
                }

                return dic;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ExtractResult()
        {
            //Tiles = new ExtractResultCollection();
            //Blocks = new ExtractResultCollection();
            //Metas = new  Dictionary<string, ExtractResult>();
        }
    }
}