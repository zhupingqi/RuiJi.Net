using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
    }

    public class ManagedCookie
    {
        private CookieContainer _container;

        public int Channel { get; set; }

        public ManagedCookie()
        {
            _container = new CookieContainer();
        }

        public string Update(string url,string setCookie)
        {
            if (setCookie == null)
                return null;
            try
            {
                var uri = new Uri(url);
                var reg = new Regex(@"expires=(.*?)[\s]GMT");
                var ms = reg.Matches(setCookie);
                setCookie = Regex.Replace(setCookie, @"expires=(.*?)[\s]GMT", "expires=Tue, 15 Jun 2038 22:57:20 GMT");
                _container.SetCookies(uri, setCookie);

                var tmp = new List<string>();

                foreach (System.Net.Cookie cookie in _container.GetCookies(uri))
                {
                    tmp.Add(cookie.Name + "=" + cookie.Value + ";expires=" + cookie.Expires.ToString("r") + "; domain=" + cookie.Domain + "; path=" + cookie.Path);
                }

                return string.Join(",", tmp.ToArray());
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public string GetCookieHeader(string url)
        {
            return _container.GetCookieHeader(new Uri(url)).ToString();
        }

        public CookieCollection GetCookie(string url)
        {
            return _container.GetCookies(new Uri(url));
        }
    }
}