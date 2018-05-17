using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net
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

        public static string FixLocalUrl(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
                return "";

            if (baseUrl.ToLower().StartsWith("localhost") || baseUrl.StartsWith("127.0.0.1"))
            {
                baseUrl = GetDefaultIPAddress().ToString() + ":" + baseUrl.Split(':')[1];
            }

            return baseUrl;
        }
    }
}