using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils
{
    public class MimeDetect
    {
        private static List<string> mimes;

        static MimeDetect()
        {
            mimes = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\text_mime.dat").ToList();
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

    public class Mimes
    {
        private static Dictionary<string, List<string>> mimes = new Dictionary<string, List<string>>();

        private static readonly string defaultType = "application/octet-stream";

        static Mimes()
        {
            var mimeTypes = Path.Combine("MimeTypes", "mimes.dat");

            using (StreamReader streamReader = new StreamReader(mimeTypes))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();

                    var currentLine = Regex.Replace(line, @"\s*#.*|^\s*|\s*$/g", "");

                    var stripWhiteSpaceRegEx = new Regex(@"\s+", RegexOptions.None);

                    if (!String.IsNullOrEmpty(currentLine))
                    {
                        var matches = stripWhiteSpaceRegEx.Split(currentLine);

                        mimes.Add(matches.First(), matches.Skip(1).ToList());
                    }
                }
            }
        }

        public static string Find(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            extension = extension.Substring(1);

            var mimeType = mimes.FirstOrDefault(x => x.Value.Exists(m => m.Contains(extension))).Key;
            if (string.IsNullOrEmpty(mimeType))
                return defaultType;

            return mimeType;
        }

        public static List<string> Extension(string mimeType)
        {
            var extensions = mimes.FirstOrDefault(x => x.Key.Equals(mimeType)).Value;
            if (extensions == null)
                return new List<string>();
            return extensions;
        }
    }
}