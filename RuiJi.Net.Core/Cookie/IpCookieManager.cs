using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace RuiJi.Net.Core.Cookie
{
    public class IpCookieManager
    {
        private static IpCookieManager _manager;
        private static object _lck = new object();

        private Dictionary<string, ManagedCookie> cookies;

        static IpCookieManager()
        {
            _manager = new IpCookieManager();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private IpCookieManager()
        {
            cookies = new Dictionary<string, ManagedCookie>();
            Reload();
        }

        public static IpCookieManager Instance
        {
            get
            {
                return _manager;
            }
        }

        public void Reload()
        {
            cookies.Clear();

            foreach (var dir in Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + @"/cookies"))
            {
                foreach (var file in Directory.GetFiles(dir))
                {
                    var json = File.ReadAllText(file, Encoding.UTF8);
                    var cookieFile = JsonConvert.DeserializeObject<CookieFile>(json);
                    var host = file.Substring(file.LastIndexOf(@"\")+1);
                    host = host.Substring(0,host.LastIndexOf("."));
                    host = "http://" + host + "/";

                    if (!cookies.ContainsKey(cookieFile.Ip))
                    {
                        var cookie = new ManagedCookie();
                        cookie.Update(host, cookieFile.Cookie);

                        cookies.Add(cookieFile.Ip, cookie);
                    }
                    else
                    {
                        cookies[cookieFile.Ip].Update(host, cookieFile.Cookie);
                    }
                }
            }
        }

        private void SaveCookieFile(string ip ,string url, string cookie)
        {
            var cookieFile = new CookieFile();
            cookieFile.Ip = ip;
            cookieFile.Cookie = cookie;

            var json = JsonConvert.SerializeObject(cookieFile);
            ip = AppDomain.CurrentDomain.BaseDirectory + @"/cookies/" + ip;
            if (!Directory.Exists(ip))
                Directory.CreateDirectory(ip);

            File.WriteAllText(ip + "/" + (new Uri(url)).Host + ".txt", json, Encoding.UTF8);
        }

        public void UpdateCookie(string ip,string url, string setCookie)
        {
            lock (_lck)
            {
                var cookie = "";

                if (cookies.ContainsKey(ip))
                {
                    cookie = cookies[ip].Update(url, setCookie);
                }
                else
                {
                    var manager = new ManagedCookie();
                    cookie = manager.Update(url, setCookie);

                    cookies.Add(ip, manager);
                }

                SaveCookieFile(ip, url, cookie);
            }
        }

        public string GetCookieHeader(string ip, string url, int channel = 0)
        {
            lock (_lck)
            {
                if (!cookies.ContainsKey(ip))
                {
                    return "";
                }

                var cookie = cookies[ip].GetCookieHeader(url);
                return cookie;
            }
        }

        public CookieCollection GetCookie(string ip, string url, int channel = 0)
        {
            lock (_lck)
            {
                if (!cookies.ContainsKey(ip))
                {
                    return new CookieCollection();
                }

                var cookie = cookies[ip].GetCookie(url);
                return cookie;
            }
        }

        public void RemoveCookie(string ip, string url, int channel = 0)
        { 
            
        }
    }
}