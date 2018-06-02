using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

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
                    var cookie = JsonConvert.DeserializeObject<ManagedCookie>(json);
                    var host = file.Substring(file.LastIndexOf(@"\")+1);
                    host = host.Substring(0,host.LastIndexOf("."));
                    host = "http://" + host + "/";
                    cookie.Update(host, cookie.Cookie);

                    if (!cookies.ContainsKey(cookie.Ip))
                        cookies.Add(cookie.Ip, cookie);
                    else
                        cookies[cookie.Ip] = cookie;
                }
            }
        }

        private void Save(string ip ,string url, ManagedCookie cookie)
        {
            var json = JsonConvert.SerializeObject(cookie);
            ip = AppDomain.CurrentDomain.BaseDirectory + @"/cookies/" + ip;
            if (!Directory.Exists(ip))
                Directory.CreateDirectory(ip);

            File.WriteAllText(ip + "/" + (new Uri(url)).Host + ".txt", json, Encoding.UTF8);
        }

        public void Update(string ip,string url, string setCookie)
        {
            lock (_lck)
            {
                if (cookies.ContainsKey(ip))
                {
                    cookies[ip].Update(url, setCookie);
                }
                else
                {
                    cookies.Add(ip, new ManagedCookie(ip, url, setCookie));
                }

                Save(ip, url, cookies[ip]);
            }
        }

        public string Get(string ip, string url)
        {
            lock (_lck)
            {
                if (!cookies.ContainsKey(ip))
                {
                    //var setCookie = SogouCookie.NewSogouCookie(path);
                    //Update(path, url, setCookie);

                    return "";
                }

                var cookie = cookies[ip].GetCookie(url);
                return cookie;
            }
        }

        public void Remove(string ip, string url)
        { 
            
        }
    }
}