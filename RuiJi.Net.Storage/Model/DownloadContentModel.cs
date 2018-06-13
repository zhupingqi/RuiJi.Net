using Newtonsoft.Json;
using RuiJi.Net.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Storage.Model
{
    public class DownloadContentModel : IContentModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("cdate")]
        public DateTime CDate { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        public bool IsRaw { get; set; }

        public object Data { get; set; }
    }
}