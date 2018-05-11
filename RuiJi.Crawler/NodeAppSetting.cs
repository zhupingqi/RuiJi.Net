using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RuiJi.Crawler
{
    public class ServerSetting
    {
        [JsonProperty("ip")]
        public string Ip
        {
            get;
            set;
        }

        [JsonProperty("port")]
        public string Port
        {
            get;
            set;
        }
    }

    public class NodeSetting
    {
        [JsonProperty("zkServer")]
        public ServerSetting ZkServer { get; set; }

        [JsonProperty("client")]
        public ServerSetting Client { get; set; }
    }
}