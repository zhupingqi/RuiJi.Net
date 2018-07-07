using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Queue
{
    /// <summary>
    /// message queue interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMessageQueue<T>
    {
        /// <summary>
        /// Add objects to the queue
        /// </summary>
        /// <param name="item"></param>
        void Enqueue(T item);

        /// <summary>
        /// Try to remove and return the object at the beginning of the concurrent queue.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        bool Dequeue(out T result);

        /// <summary>
        /// Try to return the object at the beginning of the queue, but do not remove it.
        /// </summary>
        /// <param name="result">object beginning of the queue</param>
        /// <returns>peek successful</returns>
        bool Peek(out T result);

        /// <summary>
        /// content changed handler
        /// </summary>
        /// <param name="args">args</param>
        void OnContentChanged(QueueChangedEventArgs<T> args);
    }
}
