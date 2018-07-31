using PuppeteerSharp;
using RuiJi.Net.Core.Cookie;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    public class ChromiumCrawler
    {
        /// <summary>
        /// ChromiumCrawler constructor
        /// </summary>
        public ChromiumCrawler()
        {
        }

        /// <summary>
        /// request by chromium 
        /// </summary>
        /// <param name="request">Crawl Request</param>
        /// <returns>Crawl Response</returns>
        public async Task<Response> RequestAsync(Request request)
        {
            var args = new List<string> { "--no-sandbox" };

            if (request.Proxy != null)
            {
                args.Add("--proxy-server=" + request.Proxy.Uri.ToString());
            }

            var launchOptions = new LaunchOptions { Headless = true, Args = args.ToArray() };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                launchOptions.ExecutablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromium", "chrome.exe");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                launchOptions.ExecutablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromium", "chrome");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                
            }            

            using (var browser = await Puppeteer.LaunchAsync(launchOptions))
            using (var page = await browser.NewPageAsync())
            {
                try
                {
                    //Authenticate set
                    if (!string.IsNullOrEmpty(request.Username) && !string.IsNullOrEmpty(request.Password))
                        await page.AuthenticateAsync(new Credentials { Username = request.Username, Password = request.Password });

                    //cookie set
                    var cookie = GetCookie(request);
                    var cookies = GenerateCookieParam(cookie);
                    await page.SetCookieAsync(cookies);

                    //hearder set
                    var dic = request.Headers.ToDictionary(h => h.Name, h => h.Value);
                    await page.SetExtraHttpHeadersAsync(dic);

                    //ua set
                    var ua = request.Headers.SingleOrDefault(m => m.Name == "User-Agent").Value;
                    await page.SetUserAgentAsync(ua);

                    var res = await page.GoToAsync(Uri.EscapeUriString(request.Uri.ToString()));
                    var htmlString = await page.GetContentAsync();

                    var response = new Response();
                    response.Headers = WebHeader.FromDictionary(res.Headers);
                    response.Data = htmlString;
                    response.StatusCode = res.Status;

                    return response;

                }
                catch (Exception ex)
                {
                    throw new Exception("chromium error:" + ex.Message);
                }
            }
        }

        /// <summary>
        /// generate cookie use by chromium
        /// </summary>
        /// <param name="cookie">cookie collection</param>
        /// <returns>cookie js string</returns>
        private CookieParam[] GenerateCookieParam(CookieCollection cookie)
        {
            var result = cookie.Cast<System.Net.Cookie>().Select(c => new CookieParam
            {
                Path = c.Path,
                Name = c.Name,
                Domain = c.Domain,
                Value = c.Value
            }).ToArray();

            return result;
        }

        /// <summary>
        /// get cookie by crawl request
        /// </summary>
        /// <param name="request">crawl request</param>
        /// <returns>cookie collection</returns>
        private CookieCollection GetCookie(Request request)
        {
            if (!string.IsNullOrEmpty(request.Cookie))
            {
                var c = new CookieContainer();
                c.SetCookies(request.Uri, request.Cookie);

                return c.GetCookies(request.Uri);
            }

            var ip = request.Ip;
            if (string.IsNullOrEmpty(request.Ip))
            {
                ip = IPHelper.GetDefaultIPAddress().ToString();
            }

            var ua = request.Headers.SingleOrDefault(m => m.Name == "User-Agent").Value;

            return IpCookieManager.Instance.GetCookie(ip, request.Uri.ToString(), ua);
        }
    }
}
