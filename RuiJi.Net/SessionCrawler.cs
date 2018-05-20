using RuiJi.Core.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net
{
    public class SessionCrawler : Crawler
    {
        public string Ip { get; private set; }
        public string Cookie { get; private set; }

        public SessionCrawler()
        { }

        public new Response Request(Request request)
        {
            if(!string.IsNullOrEmpty(Ip))
            {
                request.Ip = Ip;
            }

            if (!string.IsNullOrEmpty(Cookie))
                request.Cookie = Cookie;

            var resposne = base.Request(request);
            Cookie = resposne.Cookie;
            Ip = resposne.ElectInfo.Split('/')[1];

            return resposne;
        }
    }
}
