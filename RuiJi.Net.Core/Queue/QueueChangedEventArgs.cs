using System;

namespace RuiJi.Net.Core.Queue
{
    public class QueueChangedEventArgs<T> : EventArgs
    {
        public QueueChangedEventArgs(QueueChangedActionEnum action, T changedItem)
        {
            this.Action = action;
            this.ChangedItem = changedItem;
        }

        public QueueChangedEventArgs(QueueChangedActionEnum action)
        {
            this.Action = action;
        }

        public QueueChangedActionEnum Action { get; private set; }

        public T ChangedItem { get; private set; }
    }
}