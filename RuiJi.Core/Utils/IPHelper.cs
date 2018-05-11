using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Utils
{
    public class IPHelper
    {
        public static IPAddress GetDefaultIPAddress()
        {
            IPAddress[] hostIPAddress = GetHostIPAddress();
            if (hostIPAddress.Count<IPAddress>() == 0)
            {
                throw new Exception("本机没有可用ipv4地址");
            }
            return hostIPAddress[0];
        }

        public static IPAddress[] GetHostIPAddress()
        {
            var ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList.ToList();
            ips.RemoveAll(m => m.AddressFamily != AddressFamily.InterNetwork || m.ToString() == "127.0.0.1");

            return (from m in ips
                    orderby m.ToString()
                    select m).ToArray();
        }

        public static bool IsHostIPAddress(IPAddress ip)
        {
            return GetHostIPAddress().Contains<IPAddress>(ip);
        }
    }
}