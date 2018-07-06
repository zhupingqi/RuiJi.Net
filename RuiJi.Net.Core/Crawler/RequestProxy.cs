using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    /// <summary>
    /// request proxy model
    /// </summary>
    public class RequestProxy
    {
        /// <summary>
        /// proxy ip
        /// </summary>
        [JsonProperty("Ip")]
        public string Ip { get; set; }

        /// <summary>
        /// proxy port
        /// </summary>
        [JsonProperty("port")]
        public int Port { get; set; }

        /// <summary>
        /// proxy scheme http
        /// </summary>
        [JsonProperty("scheme")]
        public string Scheme { get; set; }

        /// <summary>
        /// proxy uri
        /// </summary>
        [JsonIgnore]
        public Uri Uri
        {
            get
            {
                return new Uri((string.IsNullOrEmpty(Scheme) ? "http" : Scheme) + "://" + Ip + ":" + Port);
            }
        }

        /// <summary>
        /// proxy username
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// proxy password
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public RequestProxy()
        {
            Scheme = "http";
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ip">proxy ip</param>
        /// <param name="port">proxy port</param>
        public RequestProxy(string ip, int port):this()
        {
            this.Ip = ip;
            this.Port = port;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ip">proxy ip</param>
        /// <param name="port">proxy port</param>
        /// <param name="username">proxy username</param>
        /// <param name="password">proxy password</param>
        public RequestProxy(string ip, int port, string username, string password) : this(ip, port)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}