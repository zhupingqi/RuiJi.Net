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
    public class ManagedCookie
    {
        private CookieContainer _container;

        public string UserAgent
        {
            get;
            set;
        }

        public ManagedCookie(string ua)
        {
            _container = new CookieContainer();
            this.UserAgent = ua;
        }

        public string Update(string url, string setCookie)
        {
            if (setCookie == null)
                return null;

            var uri = new Uri(url);
            var cookies = Regex.Split(setCookie, @",[^\s]");

            foreach (var cookie in cookies)
            {
                var c = Regex.Replace(cookie, @"expires=(.*?)[\s]GMT", "expires=Tue, 15 Jun 2038 22:57:20 GMT",RegexOptions.IgnoreCase);
                try
                {
                    _container.SetCookies(uri, c);
                }
                catch
                {

                }
            }

            var tmp = new List<string>();

            foreach (System.Net.Cookie cookie in _container.GetCookies(uri))
            {
                tmp.Add(cookie.Name + "=" + cookie.Value + ";expires=" + cookie.Expires.ToString("r") + "; domain=" + cookie.Domain + "; path=" + cookie.Path);
            }

            return string.Join(",", tmp.ToArray());            
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