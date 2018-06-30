using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Queue
{
    public interface IMessageQueue<T>
    {
        void Enqueue(T item);

        bool Dequeue(out T result);

        bool Peek(out T result);

        void OnContentChanged(QueueChangedEventArgs<T> args);
    }
}
