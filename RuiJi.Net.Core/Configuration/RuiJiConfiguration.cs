using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RuiJi.Net.Core.Configuration
{
    public class RuiJiConfiguration
    {
        public static List<Node> Nodes { get; private set; }

        public static bool Standalone
        {
            get
            {
                return Nodes.Count == 0;
            }
        }

        public static string RuiJiServer { get; private set; }

        public static string DocServer { get; private set; }

        public static string ZkServer { get; private set; }


        public static string ZkPath { get; private set; }

        static RuiJiConfiguration()
        {
            Nodes = new List<Node>();

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("ruiji.json");
            var config = builder.Build();

            var nodes = config.GetSection("nodes");

            foreach (var node in nodes.GetChildren())
            {
                Nodes.Add(new Node {
                    BaseUrl = node.GetSection("baseUrl").Value,
                    Type = node.GetSection("type").Value,
                    Proxy = node.GetSection("proxy").Value
                });
            }

            var setting = config.GetSection("setting");

            RuiJiServer = setting.GetSection("ruiJiServer").Value;
            DocServer = setting.GetSection("docServer").Value;
            ZkServer = setting.GetSection("zkServer").Value;
            ZkPath = setting.GetSection("zkPath").Value;
        }
    }
}