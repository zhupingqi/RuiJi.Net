using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Logging;

namespace RuiJi.Net.Core.Cookie
{
    /// <summary>
    /// cookie manager with ip
    /// </summary>
    public class IpCookieManager
    {
        private static IpCookieManager _manager;
        private static object _lck = new object();

        private Dictionary<string, ManagedCookie> cookies;

        static IpCookieManager()
        {
            Logger.GetLogger("").Info("IpCookieManager init");

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies");
            Directory.CreateDirectory(path);

            _manager = new IpCookieManager();
        }

        private IpCookieManager()
        {
            cookies = new Dictionary<string, ManagedCookie>();
            Reload();
        }

        /// <summary>
        /// IpCookieManager Instance
        /// </summary>
        public static IpCookieManager Instance
        {
            get
            {
                return _manager;
            }
        }

        /// <summary>
        /// reload cookies
        /// </summary>
        public void Reload()
        {
            Logger.GetLogger("").Info("reload cookies");

            cookies.Clear();

            foreach (var ipDir in Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies")))
            {
                foreach (var uaDir in Directory.GetDirectories(ipDir))
                {
                    foreach (var file in Directory.GetFiles(uaDir))
                    {
                        var json = File.ReadAllText(file, Encoding.UTF8);
                        var cookieFile = JsonConvert.DeserializeObject<CookieFile>(json);

                        var f = new FileInfo(file);

                        var host = f.Name;
                        host = host.Substring(0, host.LastIndexOf("."));
                        host = "http://" + host + "/";

                        if (!cookies.ContainsKey(cookieFile.Key))
                        {
                            var cookie = new ManagedCookie(cookieFile.UserAgent);
                            cookie.Update(host, cookieFile.Cookie);

                            cookies.Add(cookieFile.Key, cookie);
                        }
                        else
                        {
                            cookies[cookieFile.Key].Update(host, cookieFile.Cookie);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// save cookie on disk
        /// </summary>
        /// <param name="ip">ip address</param>
        /// <param name="ua">user agent</param>
        /// <param name="url">url</param>
        /// <param name="cookie">cookie content</param>
        private void SaveCookieFile(string ip, string ua, string url, string cookie)
        {
            var cookieFile = new CookieFile();
            cookieFile.Ip = ip;
            cookieFile.UserAgent = ua;
            cookieFile.Cookie = cookie;

            var json = JsonConvert.SerializeObject(cookieFile);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies", ip, EncryptHelper.GetMD5Hash(ua));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var f = Path.Combine(path, (new Uri(url)).Host + ".txt");

            File.WriteAllText(f, json, Encoding.UTF8);
        }

        /// <summary>
        /// update cookie
        /// </summary>
        /// <param name="ip">ip address</param>
        /// <param name="ua">user agent</param>
        /// <param name="url">url</param>
        /// <param name="setCookie">cookie content</param>
        public void UpdateCookie(string ip, string ua, string url, string setCookie)
        {
            lock (_lck)
            {
                var cookie = "";
                var key = ip + "_" + EncryptHelper.GetMD5Hash(ua);

                if (cookies.ContainsKey(key))
                {
                    cookie = cookies[key].Update(url, setCookie);
                }
                else
                {
                    var manager = new ManagedCookie(ua);
                    cookie = manager.Update(url, setCookie);

                    cookies.Add(key, manager);
                }

                SaveCookieFile(ip, ua, url, cookie);
            }
        }

        /// <summary>
        /// get cookie header 
        /// </summary>
        /// <param name="ip">ip address</param>
        /// <param name="ua">user agent</param>
        /// <param name="url">url</param>
        /// <returns>cookie content</returns>
        public string GetCookieHeader(string ip, string url, string ua)
        {
            lock (_lck)
            {
                var key = ip + "_" + EncryptHelper.GetMD5Hash(ua);

                if (!cookies.ContainsKey(key))
                {
                    return "";
                }

                var cookie = cookies[key].GetCookieHeader(url);
                return cookie;
            }
        }

        /// <summary>
        /// get cookie collection
        /// </summary>
        /// <param name="ip">ip address</param>
        /// <param name="ua">user agent</param>
        /// <param name="url">url</param>
        /// <returns>cookie collection</returns>
        public CookieCollection GetCookie(string ip, string url, string ua)
        {
            lock (_lck)
            {
                var key = ip + "_" + EncryptHelper.GetMD5Hash(ua);

                if (!cookies.ContainsKey(key))
                {
                    return new CookieCollection();
                }

                var cookie = cookies[key].GetCookie(url);
                return cookie;
            }
        }
    }
}