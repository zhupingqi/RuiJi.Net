using System;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Core.Queue
{
    public class QueueActionDelegate
    {
        public Delegate Action { get; set; }

        public object Args { get; set; }
    }
}
