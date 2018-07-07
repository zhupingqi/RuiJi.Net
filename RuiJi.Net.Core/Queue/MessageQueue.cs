using System.Collections.Concurrent;

namespace RuiJi.Net.Core.Queue
{
    public sealed class MessageQueue<T> : ConcurrentQueue<T>, IMessageQueue<T>
    {
        public event QueueChangedEventHandler<T> ContentChanged;

        /// <summary>
        /// Add objects to the queue
        /// </summary>
        /// <param name="item"></param>
        public new void Enqueue(T item)
        {
            base.Enqueue(item);

            OnContentChanged(
                new QueueChangedEventArgs<T>(QueueChangedActionEnum.Enqueue, item));
        }

        /// <summary>
        /// Try to remove and return the object at the beginning of the concurrent queue.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Try to return the object at the beginning of the queue, but do not remove it.
        /// </summary>
        /// <param name="result">object beginning of the queue</param>
        /// <returns>peek successful</returns>
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

        /// <summary>
        /// content changed handler
        /// </summary>
        /// <param name="args">args</param>
        public void OnContentChanged(QueueChangedEventArgs<T> args)
        {
            this.ContentChanged?.Invoke(this, args);
        }
    }
}