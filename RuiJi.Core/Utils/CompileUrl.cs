using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Core.Utils
{
    public class CompileUrl
    {
        public static string Compile(string url)
        {
            var regex = new Regex(@"\{#(.*?)#\}");
            var ms = regex.Matches(url);

            if (ms.Count == 0)
                return url;

            // now("yyyyMMdd")
            // ticks()
            foreach (Match m in ms)
            {
                var d = m.Value.Trim();
                var f = Regex.Match(d,@"^[^\(]*").Value.Trim();
                var arg = Regex.Match(f, @".?\((.*)\)+");
                //what... 
            }

            return "";
        }
    }
}