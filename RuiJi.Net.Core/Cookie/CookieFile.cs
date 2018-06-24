using Newtonsoft.Json;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Cookie
{
    public class CookieFile
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("cookie")]
        public string Cookie
        {
            get;
            set;
        }

        [JsonProperty("ua")]
        public string UserAgent
        {
            get;
            set;
        }

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