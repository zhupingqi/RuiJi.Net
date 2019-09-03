using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils
{
    /// <summary>
    /// wildcard match class
    /// </summary>
    public class Wildcard
    {
        /// <summary>
        /// Maximum forward matching
        /// </summary>
        /// <param name="content"></param>
        /// <param name="masks"></param>
        /// <returns></returns>
        public static string MaxMatch(string content,string[] masks)
        {
            masks = masks.Distinct().ToArray();
            var maxLength = 1;
            var result = "";

            if (Uri.IsWellFormedUriString(content, UriKind.Absolute))
            {
                var uri = new Uri(content);
                content = uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
            }

            foreach (var mask in masks)
            {
                if (CompareWildcard(content,mask))
                {
                    var length = mask.Length;
                    if (maxLength < length)
                    {
                        maxLength = length;
                        result = mask;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// match content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="masks"></param>
        /// <returns></returns>
        public static bool IsMatch(string content, string[] masks)
        {
            if (string.IsNullOrEmpty(content))
                return false;

            content = content.ToLower().Trim();

            foreach (var mask in masks)
            {
                if (CompareWildcard(content, mask))
                    return true;
            }

            return false;
        }

        private static bool CompareWildcard(IEnumerable<char> input, string mask)
        {
            mask = mask.ToLower();

            for (int i = 0; i < mask.Length; i++)
            {
                switch (mask[i])
                {
                    case '?':
                        if (!input.Any())
                            return false;

                        input = input.Skip(1);
                        break;
                    case '*':
                        while (input.Any() && !CompareWildcard(input, mask.Substring(i + 1)))
                            input = input.Skip(1);
                        break;
                    default:
                        if (!input.Any() || input.First() != mask[i])
                            return false;

                        input = input.Skip(1);
                        break;
                }
            }

            return !input.Any();
        }

        private static bool CompareWildcard(string input, string mask)
        {
            var chars = input.AsEnumerable();

            return CompareWildcard(chars, mask);
        }
    }
}