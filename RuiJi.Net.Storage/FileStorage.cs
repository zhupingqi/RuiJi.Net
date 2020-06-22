using RuiJi.Net.Storage.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RuiJi.Net.Storage
{
    public class FileStorage : StorageBase<DownloadContentModel>
    {
        public string Folder { get; private set; }

        static FileStorage()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "www", "download");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public FileStorage(string folder)
        {
            Folder = folder;
        }

        public override int Insert(DownloadContentModel content)
        {
            try
            {
                var ext = Path.GetExtension(content.Url).ToLower();
                var name = GetMD5Hash(content.Url);
                if (string.IsNullOrEmpty(ext))
                {
                    ext = ".txt";
                }

                var path = Path.Combine(Folder, name + ext);

                if (content.IsRaw)
                {
                    byte[] raw;

                    if (IsBase64(content.Data.ToString()))
                    {
                        raw = Convert.FromBase64String(content.Data.ToString());
                    }
                    else
                    {
                        raw = content.Data as byte[];
                    }

                    File.WriteAllBytes(path, raw);
                }
                else
                    File.WriteAllText(path, content.Data.ToString());
            }
            catch 
            {
            }

            return 0;
        }

        public override int Insert(DownloadContentModel[] contents)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(string url)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DownloadContentModel content)
        {
            throw new NotImplementedException();
        }

        public static string GetMD5Hash(String input)
        {
            MD5 md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private bool IsBase64(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            else
            {
                if (str.Length % 4 != 0)
                {
                    return false;
                }

                char[] strChars = str.ToCharArray();
                foreach (char c in strChars)
                {
                    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')
                        || c == '+' || c == '/' || c == '=')
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
