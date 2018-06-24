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
        [JsonProperty("Ip")]
        public string Ip { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("scheme")]
        public string Scheme { get; set; }

        [JsonIgnore]
        public Uri Uri
        {
            get
            {
                return new Uri((string.IsNullOrEmpty(Scheme) ? "http" : Scheme) + "://" + Ip + ":" + Port);
            }
        }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public RequestProxy()
        {
            Scheme = "http";
        }

        public RequestProxy(string ip, int port):this()
        {
            this.Ip = ip;
            this.Port = port;
        }

        public RequestProxy(string host, int port, string username, string password) : this(host, port)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}