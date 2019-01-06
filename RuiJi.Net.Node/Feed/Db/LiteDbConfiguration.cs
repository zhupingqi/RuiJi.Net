using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RuiJi.Net.Node.Feed.Db
{
    public class LiteDbConfiguration
    {
        public static readonly string UA;
        public static readonly string CONTENT;
        public static readonly string RULE;
        public static readonly string PROXY;
        public static readonly string FUNC;
        public static readonly string FEED;

        static LiteDbConfiguration()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("ruiji.json");
            var config = builder.Build();

            var storage = config.GetSection("storage");

            UA = storage.GetSection("ua").Value;
            RULE = storage.GetSection("rule").Value;
            PROXY = storage.GetSection("proxy").Value;
            FUNC = storage.GetSection("func").Value;
            FEED = storage.GetSection("feed").Value;
            CONTENT = storage.GetSection("content").Value;
        }
    }
}