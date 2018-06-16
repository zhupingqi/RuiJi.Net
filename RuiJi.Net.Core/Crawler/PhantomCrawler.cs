using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuiJi.Net.Core.Cookie;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    public class Res
    {
        public string contentType { get; set; }

        public List<WebHeader> headers { get; set; }

        public string charset { get; set; }

        public string cookie { get; set; }

        public int? status { get; set; }

        public string url { get; set; }
    }

    public class PhantomCrawler
    {
        private static string _js;
        private static string _tmp_js_path;

        static PhantomCrawler()
        {
            _js = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crawl.js"));
            _tmp_js_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ph_download");
        }

        public PhantomCrawler()
        {

        }

        public Response Request(Request request)
        {
            var extension = Path.GetExtension(request.Uri.ToString()).ToLower();
            var guid = ShortGUID();
            var file = @"ph_download\" + guid + extension;

            var args = "";
            if (request.Proxy != null)
            {
                var u = new Uri(request.Proxy.Host);
                args += "--proxy=" + u.Host + ":" + request.Proxy.Port + " --proxy-type=" + u.Scheme;
                if (!string.IsNullOrEmpty(request.Proxy.Username))
                    args += " " + request.Proxy.Username;
                if (!string.IsNullOrEmpty(request.Proxy.Password))
                    args += " " + request.Proxy.Password;
            }

            //phantom.addCookie({
            //  'name'     : 'Valid-Cookie-Name',   /* required property */
            //  'value'    : 'Valid-Cookie-Value',  /* required property */
            //  'domain'   : 'localhost',
            //  'path'     : '/foo',                /* required property */
            //  'httponly' : true,
            //  'secure'   : false,
            //  'expires'  : (new Date()).getTime() + (1000 * 60 * 60)   /* <-- expires in 1 hour */
            //});

            var cookies = GetCookie(request);
            if (cookies.Count > 0 && request.UseCookie)
            {
                var cookie = GetCookieJs(cookies);

                var js = _js.Replace("phantom.addCookie({});", cookie);
                var jsFile = _tmp_js_path + @"\" + guid + ".js";
                File.WriteAllText(jsFile, js);

                args += @" \ph_download\" + guid + ".js " + Uri.EscapeUriString(request.Uri.ToString()) + " " + file;
            }
            else
            {
                args += " crawl.js " + Uri.EscapeUriString(request.Uri.ToString()) + " " + file;
            }

            var p = new Process();
            p.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "phantomjs.exe");
            p.StartInfo.Arguments = args.Trim();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.RedirectStandardError = false;
            p.StartInfo.CreateNoWindow = false;
            p.Start();

            p.WaitForExit(30000);
            p.Dispose();
            p = null;

            file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);

            var response = new Response();
            if (File.Exists(file))
            {
                response.Data = File.ReadAllText(file);
                File.Delete(file);
            }

            if (File.Exists(file + ".json"))
            {
                var json = File.ReadAllText(file + ".json");
                var res = JsonConvert.DeserializeObject<Res>(json);
                response.Headers = res.headers;
                response.Charset = res.charset;
                response.ResponseUri = new Uri(res.url);
                response.StatusCode = (System.Net.HttpStatusCode)res.status.Value;
                if (!string.IsNullOrEmpty(res.contentType))
                    response.IsRaw = MimeDetect.IsRaw(res.contentType);
                else
                    response.IsRaw = MimeDetect.IsRaw(res.contentType);


                File.Delete(file + ".json");
            }

            if (File.Exists(_tmp_js_path + @"\" + guid + ".js"))
            {
                File.Delete(_tmp_js_path + @"\" + guid + ".js");
            }

            return response;
        }

        private string ShortGUID()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        public string GetCookieJs(CookieCollection cookie)
        {
            var c = cookie.Cast<System.Net.Cookie>().Select(m => "phantom.addCookie(" + JsonConvert.SerializeObject(new
            {
                name = m.Name,
                value = m.Value,
                path = m.Path,
                domain = m.Domain
            }) + ");").ToList();

            return string.Join("\r\n\r\n", c);
        }

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

            return IpCookieManager.Instance.GetCookie(ip, request.Uri.ToString());
        }

        private void SetCookie(Request request, string setCookie)
        {
            if (string.IsNullOrEmpty(setCookie))
                return;

            var ip = request.Ip;
            if (string.IsNullOrEmpty(request.Ip))
            {
                ip = IPHelper.GetDefaultIPAddress().ToString();
            }

            IpCookieManager.Instance.UpdateCookie(ip, request.Uri.ToString(), setCookie);
        }
    }
}