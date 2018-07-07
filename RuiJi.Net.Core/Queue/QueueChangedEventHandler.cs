namespace RuiJi.Net.Core.Queue
{
    /// <summary>
    /// queue changed event handler
    /// </summary>
    /// <typeparam name="T">item type</typeparam>
    /// <param name="sender">event send object</param>
    /// <param name="args">queue changed event arguments</param>
    public delegate void QueueChangedEventHandler<T>(
        object sender, 
        QueueChangedEventArgs<T> args);
}