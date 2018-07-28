using RuiJi.Net.Core.Utils.Suffix;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// uri extensions
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// get uri domain
        /// </summary>
        /// <param name="uri">this</param>
        /// <returns>doamin</returns>
        public static string GetDomain(this Uri uri)
        {
            return DomainParser.Parser.Parse(uri.Host).RegistrableDomain;
        }

        /// <summary>
        /// Maximum same segments length
        /// </summary>
        /// <param name="uri">this</param>
        /// <param name="target">target uri</param>
        /// <returns></returns>
        public static int StartSegmentsWith(this Uri uri, Uri target)
        {
            var sp1 = uri.AbsolutePath.Split('/');
            var sp2 = target.AbsolutePath.Split('/');

            var count = 0;
            var length = Math.Min(sp1.Length, sp2.Length);

            for (int i = 0; i < length; i++)
            {
                if (sp1[i] == sp2[i])
                    count++;
                else
                    break;
            }

            return count;
        }

        /// <summary>
        /// Whether or not it contains the target address path
        /// </summary>
        /// <param name="uri">uri</param>
        /// <param name="target">target uri</param>
        /// <returns>result</returns>
        public static bool StartPathWith(this Uri uri, Uri target)
        {
            var sp1 = uri.AbsolutePath.Split('/');
            var sp2 = target.AbsolutePath.Split('/');

            var count = 0;
            var length = Math.Min(sp1.Length, sp2.Length);

            for (int i = 0; i < length; i++)
            {
                if (sp1[i] == sp2[i])
                    count++;
                else
                    break;
            }

            return count == sp2.Length;
        }

        /// <summary>
        /// wildcard compare
        /// </summary>
        /// <param name="uri">this</param>
        /// <param name="mask">mask</param>
        /// <returns>compare result</returns>
        public static bool WildcardMatch(this Uri uri, string mask)
        {
            return CompareWildcard((uri.Scheme + "://" + uri.Host + uri.AbsolutePath).AsEnumerable(), mask);
        }

        /// <summary>
        /// wildcard compare
        /// </summary>
        /// <param name="input">char enumerable</param>
        /// <param name="mask">wildcard</param>
        /// <returns>compare result</returns>
        private static bool CompareWildcard(IEnumerable<char> input, string mask)
        {
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
    }
}
