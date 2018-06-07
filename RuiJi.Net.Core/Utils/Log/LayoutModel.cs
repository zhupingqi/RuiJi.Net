using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils.Log
{
    public class LayoutModel
    {

        public LayoutModel(string container = "%date [%thread] %-5level - %message%newline", string header = "------ New session ------", string footer = "------ End session ------")
        {
            Container = container;
            Header = header;
            Footer = footer;
        }

        public string Container { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
    }
}
