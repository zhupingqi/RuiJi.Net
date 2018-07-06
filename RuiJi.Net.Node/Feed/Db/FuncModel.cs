using Newtonsoft.Json;
using RuiJi.Net.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed.Db
{
    public class FuncModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(EnumConvert<FuncType>))]
        public FuncType Type { get; set; }

        [JsonProperty("sample")]
        public string Sample { get; set; }
    }
}
