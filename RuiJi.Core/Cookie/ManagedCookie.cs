using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Core.Cookie
{
    public class ManagedCookie
    {
        public string Ip { get; set; }

        private string _setCookie;
        public string SetCookie
        {
            get
            {
                return _setCookie;
            }
            private set
            {
                _setCookie = value;
            }
        }

        private CookieContainer _container;

        public ManagedCookie()
        {
            _container = new CookieContainer();
        }

        public ManagedCookie(string ip ,string url, string setCookie)
            : this()
        {
            if (ip.IndexOf("/") != -1)
                throw new ArgumentException("路径包含非法字符");

            this.Ip = ip;

            Update(url,setCookie);
        }

        public void Update(string url,string setCookie)
        {
            if (setCookie == null)
                return;

            var uri = new Uri(url);
            var reg = new Regex(@"(expires=[^,]*,[\s]*[\d]*-[^-]*-)([\d]*)([\s]*[\d]*:[\d]*:[\d]*[\s]GMT)");
            var ms = reg.Matches(setCookie);
            foreach (Match m in ms)
            {
                string f0 = m.Groups[0].Value;
                string f1 = m.Groups[1].Value;
                string f2 = m.Groups[2].Value;
                string f3 = m.Groups[3].Value;

                if (f2.Length == 2)
                    setCookie = setCookie.Replace(f0, f1 + "20" + f2 + f3);
            }

            _container.SetCookies(uri, setCookie);

            var tmp = new List<string>();

            foreach (System.Net.Cookie cookie in _container.GetCookies(uri))
            {
                tmp.Add(cookie.Name + "=" + cookie.Value + ";expires=" + cookie.Expires.ToString("r") + "; domain=" + cookie.Domain + "; path=" + cookie.Path);
            }

            _setCookie = string.Join(",", tmp.ToArray());
        }

        public string GetCookie(string url)
        {
            return _container.GetCookieHeader(new Uri(url)).ToString();
        }
    }
}