using RuiJi.Net.Owin.SysStatus;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RuiJi.Net.Owin.SysStatus
{
    public class SystemStatusManager
    {
        public static SystemStatus Instance { get; set; }

        static SystemStatusManager()
        {
            Instance = CreateSystemStatus();
        }

        private static SystemStatus CreateSystemStatus()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxSystemStatus();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new OSXSystemStatus();
            }

            return new WindowsSystemStatus();
        }
    }
}
