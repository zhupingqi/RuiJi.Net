using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Suffix
{
    public class DomainParser
    {
        internal static readonly IdnMapping IdnMapping = new IdnMapping();

        private static string datUrl = "https://publicsuffix.org/list/effective_tld_names.dat";
        private static string fileName = "effective_tld_names.dat";
        private static DomainParser cacheParser;

        private readonly Rule _rule;

        static DomainParser()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + fileName))
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead(datUrl))
                {
                    var reader = new StreamReader(stream);

                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + fileName, reader.ReadToEnd());
                }
            }
        }

        private DomainParser(Rule rule)
        {
            _rule = rule;
        }

        public Domain Parse(string host)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            var labels = host.Split('.');

            if (labels.Any(String.IsNullOrEmpty))
                throw new ArgumentException("host");

            Array.Reverse(labels);

            return Parse(labels);
        }

        private Domain Parse(string[] labels)
        {
            var rule = _rule.FindMatchingRule(labels);

            return rule.Parse(labels);
        }

        public static DomainParser FromFile(string fileName)
        {
            using (var reader = File.OpenText(fileName))
            {
                return Read(reader);
            }
        }

        public static Task<DomainParser> FromFileAsync(string fileName)
        {
            using (var reader = File.OpenText(fileName))
            {
                return ReadAsync(reader);
            }
        }

        public static DomainParser Default
        {
            get { return FromUrl(new Uri("datUrl")); }
        }

        public static DomainParser FromUrl(Uri uri)
        {
            using (var client = new WebClient())
            using (var stream = client.OpenRead(uri))
            {
                return FromStream(stream);
            }
        }

        public static async Task<DomainParser> FromUrlAsync(Uri uri)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(uri))
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                return await FromStreamAsync(stream);
            }
        }

        public static DomainParser FromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return Read(reader);
            }
        }
        
        public static Task<DomainParser> FromStreamAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return ReadAsync(reader);
            }
        }

        private static DomainParser Read(TextReader reader)
        {
            string line;
            var lines = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                var l = line.Trim();
                if (l.Length == 0 || l.StartsWith("//")) continue;

                lines.Add(l);
            }
            
            return CreateDomainParser(lines);
        }

        private static async Task<DomainParser> ReadAsync(TextReader reader)
        {
            string line;
            var lines = new List<string>();
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var l = line.Trim();
                if (l.Length == 0 || l.StartsWith("//")) continue;

                lines.Add(l);
            }

            return CreateDomainParser(lines);
        }

        private static DomainParser CreateDomainParser(IEnumerable<string> lines)
        {
            return new DomainParser(new Rule(1, ToRuleMap(lines.Select(ParseRuleDefinition), 0)));
        }

        private static IDictionary<string, Rule> ToRuleMap(IEnumerable<RuleDefinition> rules, int index)
        {
            return rules.GroupBy(r => IdnMapping.GetAscii(r.Labels[index]))
                .ToDictionary(
                    g => g.Key,
                    g =>
                        new Rule(
                            g.Where(r => r.Labels.Length == index + 1).Select(r => r.Length).SingleOrDefault(),
                            ToRuleMap(g.Where(r => r.Labels.Length > index + 1), index + 1)),
                    StringComparer.OrdinalIgnoreCase);
        }

        private static string[] GetLabels(string name)
        {
            var labels = Array.ConvertAll(name.Split('.'), String.Intern);
            Array.Reverse(labels);
            return labels;
        }

        private static RuleDefinition ParseRuleDefinition(string rule)
        {
            if (!rule.StartsWith("!"))
            {
                var labels = GetLabels(rule);
                return new RuleDefinition(labels, labels.Length);
            }
            else
            {
                var labels = GetLabels(rule.Substring(1));
                return new RuleDefinition(labels, labels.Length - 1);
            }
        }

        public static DomainParser Parser
        {
            get
            {
                

                if (cacheParser == null)
                {
                    cacheParser = FromFile(AppDomain.CurrentDomain.BaseDirectory + fileName);
                }

                return cacheParser;
            }
        }
    }
}