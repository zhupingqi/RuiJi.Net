using System.Collections.Concurrent;

namespace RuiJi.Net.Core.Queue
{
    public sealed class MessageQueue<T> : ConcurrentQueue<T>, IMessageQueue<T>
    {
        public event QueueChangedEventHandler<T> ContentChanged;

        public new void Enqueue(T item)
        {
            base.Enqueue(item);

            OnContentChanged(
                new QueueChangedEventArgs<T>(QueueChangedActionEnum.Enqueue, item));
        }

        public bool Dequeue(out T result)
        {
            if (!TryDequeue(out result))
            {
                return false;
            }

            OnContentChanged(
                new QueueChangedEventArgs<T>(QueueChangedActionEnum.Dequeue, result));

            if (IsEmpty)
            {
                OnContentChanged(
                    new QueueChangedEventArgs<T>(QueueChangedActionEnum.Empty));
            }

            return true;
        }

        public bool Peek(out T result)
        {
            var retValue = TryPeek(out result);
            if (retValue)
            {
                OnContentChanged(
                    new QueueChangedEventArgs<T>(QueueChangedActionEnum.Peek, result));
            }

            return retValue;
        }

        public void OnContentChanged(QueueChangedEventArgs<T> args)
        {
            this.ContentChanged?.Invoke(this, args);
        }
    }
}