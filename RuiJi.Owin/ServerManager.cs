using Microsoft.Owin.Hosting;
using RuiJi.Node;
using RuiJi.Node.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Owin
{
    public class ServerManager
    {
        private static ServerManager manager;
        private List<WebApiServer> servers;

        static ServerManager()
        {
            manager = new ServerManager();
        }

        private ServerManager()
        {
            servers = new List<WebApiServer>();
        }

        ~ServerManager()
        {
            Stop();
        }

        public static ServerManager Inst
        {
            get
            {
                return manager;
            }
        }

        public void Start(NodeConfigurationElement config)
        {
            var server = new WebApiServer();
            server.Start(config.BaseUrl, config.Type, config.ZkServer, config.Proxy);

            servers.Add(server);
        }

        public void Stop(string port = "")
        {
            if (string.IsNullOrEmpty(port))
            {
                servers.ForEach(m =>
                {
                    m.Stop();
                    Console.WriteLine("server port with " + m.Port + " stop!");
                });
                servers.Clear();

                Console.WriteLine("all server stop!");
            }
            else
            {
                var server = servers.SingleOrDefault(m => m.Port == port);
                if (server != null)
                {
                    server.Stop();
                    servers.Remove(server);

                    Console.WriteLine("server port with " + port + " stop!");
                }
            }
        }

        internal NodeBase GetNode(string port)
        {
            var server = servers.SingleOrDefault(m => m.Port == port);
            if (server != null)
            {
                return server.NodeBase;
            }

            return null;
        }
    }
}