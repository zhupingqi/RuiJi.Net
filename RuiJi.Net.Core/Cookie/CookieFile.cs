using Newtonsoft.Json;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Cookie
{
    /// <summary>
    /// cookie persistence model
    /// </summary>
    public class CookieFile
    {
        /// <summary>
        /// ip address 
        /// </summary>
        [JsonProperty("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// cookie content
        /// </summary>
        [JsonProperty("cookie")]
        public string Cookie
        {
            get;
            set;
        }

        /// <summary>
        /// user agent
        /// </summary>
        [JsonProperty("ua")]
        public string UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// cookie unique identifier
        /// </summary>
        [JsonIgnore]
        public string Key
        {
            get
            {
                return Ip + "_" + EncryptHelper.GetMD5Hash(UserAgent);
            }
        }

        public CookieFile()
        {            
        }
    }
}