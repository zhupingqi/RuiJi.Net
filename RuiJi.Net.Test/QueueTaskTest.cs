using RuiJi.Net.Core.Queue;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace RuiJi.Net.Test
{

    public class QueueTaskTest
    {
        [Fact]
        public void TestMethod1()
        {
            var pool = new TaskQueuePool(4);
            pool.Start();

            pool.QueueAction((f) => {
                Thread.Sleep(100);

                Debug.Write(" first");
                Debug.Write(" queue count : " + pool.Count);
                Debug.WriteLine(" current tasks : " + pool.CurrentTasks);
            },"first");

            Thread.Sleep(1000);

            for (int i = 0; i < 10; i++)
            {
                pool.QueueAction((index) => {
                    Thread.Sleep(1000);

                    Debug.WriteLine("aaaa " + index + " queue count : " + pool.Count + " current tasks : " + pool.CurrentTasks);
                }, i);
            }

            for (int i = 10; i < 20; i++)
            {
                pool.QueueAction((index) => {
                    Thread.Sleep(2000);

                    Debug.WriteLine("bbbb " + index + " queue count : " + pool.Count + " current tasks : " + pool.CurrentTasks);
                }, i);
            }

            Thread.Sleep(10000);

            for (int i = 20; i < 50; i++)
            {
                pool.QueueAction((index) => {
                    Thread.Sleep(500);

                    Debug.WriteLine("cccc " + index + " queue count : " + pool.Count + " current tasks : " + pool.CurrentTasks);
                }, i);
            }

            Thread.Sleep(1000000);
            Assert.True(true);
        }
    }
}
