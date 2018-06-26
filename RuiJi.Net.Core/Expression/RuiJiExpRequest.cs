using RuiJi.Net.Core.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Expression
{
    public class RuiJiExpRequest : Request
    {
        public string Identify { get; set; }

        public string CornExpression { get; set; }

        public int Delay { get; set; }
    }
}