using Microsoft.AspNetCore.Hosting;
using RuiJi.Net.Node;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Owin
{
    public interface IServer
    {
        string BaseUrl { get; }

        IWebHost WebHost { get; }

        INode Node { get; }

        int Port { get; }

        void Start();

        void Stop();
    }
}
