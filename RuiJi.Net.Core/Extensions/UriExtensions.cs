using RuiJi.Net.Core.Utils.Suffix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extensions
{
    namespace System
    {
        public static class UriExtensions
        {
            public static string GetDomain(this Uri uri)
            {
                return DomainParser.Parser.Parse(uri.Host).RegistrableDomain;
            }

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

            public static bool WildcardMatch(this Uri uri, string mask)
            {
                return CompareWildcard((uri.Scheme + "://" + uri.Host + uri.AbsolutePath).AsEnumerable(), mask);
            }

            /// <summary>
            /// Internal matching algorithm.
            /// </summary>
            /// <param name="wildcard">The wildcard.</param>
            /// <param name="s">The s.</param>
            /// <param name="wildcardIndex">Index of the wildcard.</param>
            /// <param name="sIndex">Index of the s.</param>
            /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
            /// <returns></returns>
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
}
