namespace RuiJi.Core.Queue
{
    public delegate void QueueChangedEventHandler<T>(
        object sender, 
        QueueChangedEventArgs<T> args);
}