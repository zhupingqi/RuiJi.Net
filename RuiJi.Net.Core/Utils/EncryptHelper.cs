using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace RuiJi.Net.Core.Utils
{
    /// <summary>
    /// 加密解密类
    /// </summary>
    public class EncryptHelper
    {
        private static readonly string key = "ruiji.net@zhupingqi";

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decrypt_DES(string str)
        {
            System.Security.Cryptography.TripleDESCryptoServiceProvider des = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
            int x;
            byte[] inputByteArray = new byte[str.Length / 2];
            for (x = 0; x < str.Length / 2; x++)
                inputByteArray[x] = (byte)(Convert.ToInt32(str.Substring(x * 2, 2), 16));
            des.Key = Convert.FromBase64String(GetMD5Hash(key));
            des.IV = Convert.FromBase64String("mjyxT92CmbQ=");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return System.Text.Encoding.Unicode.GetString(ms.ToArray());
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encrypt_DES(string str)
        {
            System.Security.Cryptography.TripleDESCryptoServiceProvider des = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
            byte[] inputByteArray = System.Text.Encoding.Unicode.GetBytes(str);
            des.Key = Convert.FromBase64String(GetMD5Hash(key));
            des.IV = Convert.FromBase64String("mjyxT92CmbQ=");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (byte b in ms.ToArray())
                sb.AppendFormat("{0:X2}", b);
            return sb.ToString();
        }
        
        public static string GetMD5Hash(String input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}