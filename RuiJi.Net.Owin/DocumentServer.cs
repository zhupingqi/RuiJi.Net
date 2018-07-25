using Microsoft.AspNetCore.Hosting;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Node;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Owin
{
    public class DocumentServer : IServer
    {
        public string BaseUrl { get; private set; }

        public IWebHost WebHost { get; private set; }

        public INode Node { get; private set; }

        public int Port { get; private set; }

        public DocumentServer(string baseUrl)
        {
            BaseUrl = IPHelper.FixLocalUrl(baseUrl);

            Port = 80;
            if (BaseUrl.IndexOf(":") != -1)
                Port = Convert.ToInt32(BaseUrl.Split(':')[1]);
        }

        public void Start()
        {
            WebHost = new WebHostBuilder()
                .UseKestrel()
                .UseWebRoot(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "document"))
                .UseUrls("http://" + BaseUrl)
                .UseIISIntegration()
                .UseStartup<DocStartup>()
                .Build();

            WebHost.RunAsync();
        }

        public void Stop()
        {
            if (WebHost != null)
            {
                WebHost.StopAsync(TimeSpan.FromSeconds(0));
                WebHost.Dispose();
                WebHost = null;
            }
        }
    }
}