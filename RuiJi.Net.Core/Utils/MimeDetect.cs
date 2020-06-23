using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils
{
    /// <summary>
    /// use to mime detect
    /// </summary>
    public class MimeDetect
    {
        private static List<string> mimes;

        static MimeDetect()
        {
            var path = new FileInfo(typeof(MimeDetect).Assembly.Location).Directory.FullName;
            mimes = File.ReadAllLines(Path.Combine(path, "text_mime.dat")).ToList();
        }

        public static bool IsRaw(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return true;

            if (contentType.IndexOf(";") != -1)
                contentType = contentType.Substring(0, contentType.IndexOf(";")).Trim();

            return !mimes.Contains(contentType);
        }

        public static bool IsRaw(Uri uri)
        {
            var contentType = Mimes.Find(uri.AbsolutePath);

            return !mimes.Contains(contentType);
        }
    }    
}