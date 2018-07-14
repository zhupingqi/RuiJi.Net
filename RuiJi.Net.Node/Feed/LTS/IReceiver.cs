using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Feed.LTS
{
    public enum BroadcastEventEnum
    {
        UPDATE,
        REMOVE,
        STATUSCHANGE
    }

    public class BroadcastEvent : EventArgs
    {
        public BroadcastEventEnum Event { get; set; }


    }

    public interface IReceiver
    {
        void OnReceive(string action);
    }
}
