using System;

namespace RuiJi.Net.Core.Queue
{
    /// <summary>
    /// queue changed event arguments
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="action">queue changed action enum</param>
        /// <param name="changedItem">changed item</param>
        public QueueChangedEventArgs(QueueChangedActionEnum action, T changedItem)
        {
            this.Action = action;
            this.ChangedItem = changedItem;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="action">queue changed action enum</param>
        public QueueChangedEventArgs(QueueChangedActionEnum action)
        {
            this.Action = action;
        }

        /// <summary>
        /// queue changed action enum
        /// </summary>
        public QueueChangedActionEnum Action { get; private set; }

        /// <summary>
        /// queue changed item
        /// </summary>
        public T ChangedItem { get; private set; }
    }
}