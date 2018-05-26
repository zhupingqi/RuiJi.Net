using System.Collections.Concurrent;

namespace RuiJi.Core.Queue
{
    public sealed class MessageQueue<T> : ConcurrentQueue<T>
    {
        public event QueueChangedEventHandler<T> ContentChanged;

        public new void Enqueue(T item)
        {
            base.Enqueue(item);

            this.OnContentChanged(
                new QueueChangedEventArgs<T>(QueueChangedActionEnum.Enqueue, item));
        }

        public new bool TryDequeue(out T result)
        {
            if (!base.TryDequeue(out result))
            {
                return false;
            }

            this.OnContentChanged(
                new QueueChangedEventArgs<T>(QueueChangedActionEnum.Dequeue, result));

            if (this.IsEmpty)
            {
                this.OnContentChanged(
                    new QueueChangedEventArgs<T>(QueueChangedActionEnum.Empty));
            }

            return true;
        }

        public new bool TryPeek(out T result)
        {
            var retValue = base.TryPeek(out result);
            if (retValue)
            {
                this.OnContentChanged(
                    new QueueChangedEventArgs<T>(QueueChangedActionEnum.Peek, result));
            }

            return retValue;
        }

        private void OnContentChanged(QueueChangedEventArgs<T> args)
        {
            var handler = this.ContentChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}