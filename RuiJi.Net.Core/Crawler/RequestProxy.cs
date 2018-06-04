using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    public class RequestProxy
    {
        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public RequestProxy()
        {

        }

        public RequestProxy(string host,int port)
        {
            this.Host = host;
            this.Port = port;
        }

        public RequestProxy(string host, int port,string username,string password): this(host, port)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}