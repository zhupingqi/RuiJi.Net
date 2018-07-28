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
    /// <summary>
    /// managed cookie
    /// </summary>
    public class ManagedCookie
    {
        private CookieContainer _container;

        /// <summary>
        /// user agent
        /// </summary>
        public string UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// managed cookie constructor
        /// </summary>
        /// <param name="ua">user agent</param>
        public ManagedCookie(string ua)
        {
            _container = new CookieContainer();
            this.UserAgent = ua;
        }

        /// <summary>
        /// update cookie by url
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="setCookie">cookie content</param>
        /// <returns>cookie content result</returns>
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

        /// <summary>
        /// get cookie header
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>cookie content</returns>
        public string GetCookieHeader(string url)
        {
            return _container.GetCookieHeader(new Uri(url)).ToString();
        }

        /// <summary>
        /// get cookie collection
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>cookie collection</returns>
        public CookieCollection GetCookie(string url)
        {
            return _container.GetCookies(new Uri(url));
        }
    }
}