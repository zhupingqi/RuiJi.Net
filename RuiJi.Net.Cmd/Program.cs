using RuiJi.Net.Core.Utils.Logging;
using RuiJi.Net.Owin;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace RuiJi.Net.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
//            
//            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
//            {
//                Console.WriteLine("Name: " + netInterface.Name);
//                Console.WriteLine("Description: " + netInterface.Description);
//                Console.WriteLine("Addresses: ");
//                IPInterfaceProperties ipProps = netInterface.GetIPProperties();
//                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
//                {
//                    Console.WriteLine(" " + addr.Address.ToString());
//                }
//                Console.WriteLine("");
//            }
            
            
            Logger.GetLogger("").Info("Program started!");

            ServerManager.StartServers();

            while (true)
            {
                Console.WriteLine("please e to exit!");
                var key = Console.ReadLine();

                if (key != null && key.ToLower() == "e")
                {
                    ServerManager.StopAll();
                    break;
                }
            }

            Process.GetCurrentProcess().Kill();
        }
    }
}