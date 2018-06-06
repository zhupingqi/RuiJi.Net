using Newtonsoft.Json;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.NodeVisitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed
{
    public class ContentModel : IContentModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("feedId")]
        public int FeedId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("metas")]
        public Dictionary<string,string> Metas { get; set; }
    }

    public class FeedProxyStorage : IStorage<ContentModel>
    {
        public bool Save(ContentModel t)
        {
            return Feeder.SaveContent(t);
        }
    }
}