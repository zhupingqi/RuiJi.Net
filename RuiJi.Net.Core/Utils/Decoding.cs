using ICSharpCode.SharpZipLib.GZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ude;

namespace RuiJi.Net.Core.Utils
{
    /// <summary>
    /// decompress stream and detect content charset
    /// </summary>
    public class Decoding
    {
        private static List<string> encodings;

        static Decoding()
        {
            encodings = System.Text.Encoding.GetEncodings().Select(m => m.Name.ToLower()).ToList();
            encodings.Add("gbk");
        }

        public static byte[] Decompress(byte[] data)
        {
            using (MemoryStream oms = new MemoryStream())
            {
                MemoryStream ims = new MemoryStream(data);
                GZipStream gZip = new GZipStream(ims, CompressionMode.Decompress);
                byte[] buffer = new byte[0x400];

                while (true)
                {
                    int count = gZip.Read(buffer, 0, buffer.Length);
                    if (count <= 0)
                    {
                        gZip.Close();
                        ims.Close();

                        return oms.ToArray();
                    }
                    oms.Write(buffer, 0, count);
                }
            }
        }

        public static byte[] DecompressDeflate(byte[] buff)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                MemoryStream stream2 = new MemoryStream(buff);
                DeflateStream stream3 = new DeflateStream(stream2, CompressionMode.Decompress);
                stream3.CopyTo(stream);
                stream2.Close();
                stream3.Close();
                return stream.ToArray();
            }
        }

        public static byte[] DecompressGZip(byte[] buff)
        {
            byte[] buffer;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    MemoryStream baseInputStream = new MemoryStream(buff);
                    GZipInputStream stream3 = new GZipInputStream(baseInputStream);
                    stream3.CopyTo(stream);
                    baseInputStream.Close();
                    stream3.Close();
                    buffer = stream.ToArray();
                }
            }
            catch
            {
                buffer = Decompress(buff);
            }
            return buffer;
        }

        public static DecodeResult GetStringFromBuff(byte[] buff, HttpWebResponse response, string charset = null)
        {
            if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip"))
            {
                buff = DecompressGZip(buff);
            }
            if (response.ContentEncoding != null && response.ContentEncoding.Equals("deflate"))
            {
                buff = DecompressDeflate(buff);
            }

            charset = charset ?? DetectEncoding(buff, response);

            try
            {
                var encoding = System.Text.Encoding.GetEncoding(charset);
                var body = encoding.GetString(buff);

                if (body.Length > 0 && body[0] == 0xfeff)
                {
                    body = body.Replace((char)65279, ' ').Trim();
                }

                return new DecodeResult(charset, body);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string DetectEncoding(byte[] decompressbuff, HttpWebResponse response)
        {
            var ms = new MemoryStream(decompressbuff);
            var cdet = new CharsetDetector();
            cdet.Feed(ms);
            cdet.DataEnd();
            ms.Close();

            var encoding = cdet.Charset;
            if (encoding != null)
                return encoding;

            encoding = response.CharacterSet;
            if (!String.IsNullOrEmpty(encoding) && encoding.IndexOf(',') != -1)
            {
                encoding = encoding.Split(',')[0];
            }

            if (response.ResponseUri.ToString().EndsWith(".xml") || (response.ContentType == "text/xml"))
            {
                encoding = GetXmlEncoding(decompressbuff);
            }
            else if (string.IsNullOrEmpty(encoding) || (string.Compare(encoding, "ISO-8859-1") == 0))
            {
                encoding = GetEncodingFromBuffer(decompressbuff);
            }

            if (!string.IsNullOrEmpty(encoding) && encoding.ToLower() == "gbk")
                encoding = "GB18030";

            if (!CharsetExists(encoding))
            {
                encoding = "utf-8";
            }

            return encoding;
        }

        public static string GetEncodingFromBuffer(byte[] buffer)
        {
            Regex regex = new Regex("<meta[^>]+?charset[\\s]*=[\\s|\"]*(?<charset>[a-zA-Z0-9-]+?)[\"|\\s\\/]{1}[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string input = System.Text.Encoding.ASCII.GetString(buffer);
            System.Text.RegularExpressions.Match match = regex.Match(input);
            if (match.Success)
            {
                return match.Groups["charset"].Value;
            }

            return null;
        }

        public static bool CharsetExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return encodings.Exists(m => m == name.ToLower());
        }

        private static string GetXmlEncoding(byte[] buffer)
        {
            var regex = new Regex("<?xml[^>]*encoding=\"(?<charset>[a-zA-Z0-9-]+?)\"[^>]*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string input = System.Text.Encoding.ASCII.GetString(buffer);
            System.Text.RegularExpressions.Match match = regex.Match(input);
            if (match.Success)
            {
                return match.Groups["charset"].Value;
            }

            return System.Text.Encoding.UTF8.BodyName;
        }
    }
}
