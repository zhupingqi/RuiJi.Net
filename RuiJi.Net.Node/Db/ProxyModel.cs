using Newtonsoft.Json;
using RuiJi.Net.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Db
{
    public enum ProxyTypeEnum
    {
        HTTP,
        HTTPS
    }

    public class ProxyModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(EnumConvert<ProxyTypeEnum>))]
        public ProxyTypeEnum Type { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(EnumConvert<Status>))]
        public Status Status { get; set; }
    }
}