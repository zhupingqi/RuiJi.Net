using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Expression
{
    public class RuiJiParser
    {
        static string[] headers = {
            "[address]",
            "[feed]",
            "[rule]"
        };

        public Request Request { get; private set; }

        public ExtractFeatureBlock ExtractBlock { get; private set; }

        public RuiJiParser()
        {

        }

        public void ParseFile(string file)
        {
            if (!File.Exists(file))
                throw new Exception("file not find");

            Parse(File.ReadAllText(file));
        }

        public void Parse(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new Exception("expression is empty");

            Parse(expression.Replace("\r\n", "\n").Split('\n'));
        }

        private void Parse(string[] lines)
        {
            if (lines.Length < 3 || lines[0] != "##request" || !headers.Contains(lines[1]) || !Uri.IsWellFormedUriString(lines[2], UriKind.Absolute))
                throw new Exception("expression header does not conform to specification");

            var sections = new Dictionary<string, List<string>>();
            var key = "";

            foreach (var line in lines)
            {
                if (line.StartsWith("##"))
                {
                    sections.Add(line, new List<string>());
                    key = line;
                    continue;
                }

                sections[key].Add(line);
            }

            foreach (var secKey in sections.Keys)
            {
                var expression = string.Join("\n", sections[secKey]);

                switch (secKey)
                {
                    case "##request":
                        {
                            ParseRequest(expression);

                            break;
                        }
                    case "##extract":
                        {
                            ParseExtract(expression);
                            break;
                        }
                    case "##storage":
                        {
                            ParseStorage(expression);
                            break;
                        }
                }
            }
        }

        public Request ParseRequest(string expression)
        {
            Request = new Request();

            var jObj = JObject.FromObject(Request);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(expression));
            var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    var name = line.TrimStart('[').TrimEnd(']');
                    if (name == "address")
                        name = "uri";

                    var property = jObj.Property(name);
                    if (property != null)
                    {
                        line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        switch (name)
                        {
                            case "proxy":
                                {
                                    var sp = line.Split(' ');
                                    if (sp.Length < 2)
                                        throw new Exception("proxy must set with ip and port");

                                    var proxy = new RequestProxy(sp[0], Convert.ToInt32(sp[1]));
                                    if (sp.Length > 2)
                                        proxy.Scheme = sp[2];
                                    if (sp.Length > 3)
                                        proxy.Username = sp[3];
                                    if (sp.Length > 4)
                                        proxy.Password = sp[4];

                                    property.Value = JToken.FromObject(proxy);
                                    break;
                                }
                            case "headers":
                                {
                                    var headers = new List<WebHeader>();
                                    while (!string.IsNullOrEmpty(line) && !reader.EndOfStream)
                                    {
                                        var sp = line.Split(':');
                                        if (sp.Length < 2)
                                        {
                                            throw new Exception("header is not expected");
                                        }

                                        headers.Add(new WebHeader(line.Substring(0, line.IndexOf(':')), line.Substring(line.IndexOf(':') + 1)));

                                        line = reader.ReadLine();
                                    }

                                    if (headers.Count > 0)
                                        property.Value = JToken.FromObject(headers);

                                    break;
                                }
                            case "data":
                                {
                                    var data = "";
                                    while (!string.IsNullOrEmpty(line) && !reader.EndOfStream)
                                    {
                                        data += "\n" + line;

                                        line = reader.ReadLine();
                                    }

                                    data = data.Trim();
                                    if (data.StartsWith("{") && data.EndsWith("}"))
                                        property.Value = JToken.FromObject(JsonConvert.DeserializeObject<object>(data));
                                    else
                                        property.Value = data;


                                    break;
                                }
                            default:
                                {
                                    property.Value = line;
                                    break;
                                }
                        }
                    }
                }
            }

            Request = jObj.ToObject<Request>();

            reader.Close();
            stream.Close();

            return Request;
        }

        public ExtractFeatureBlock ParseExtract(string expression)
        {
            var block = RuiJiExtractBlockParser.ParserBlock(expression);

            ExtractBlock = new ExtractFeatureBlock(block,"") ;

            return ExtractBlock;
        }

        private void ParseStorage(string expression)
        {

        }
    }
}