using RuiJi.Cmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Test
{
    public class Common
    {
        private static List<Thread> threads = new List<Thread>();

        ~Common()
        {
            foreach (var t in threads)
            {
                t.Abort();
            }
        }

        public static void StartupNodes()
        {
            var t = new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "localhost:36000",
                "-t",
                "cp",
                "-z",
                "localhost:2181"
                };
                Program.Main(args);
            });
            t.Start();
            threads.Add(t);

            // start crawler
            t = new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "localhost:36001",
                "-t",
                "c",
                "-p",
                "localhost:36000",
                "-z",
                "localhost:2181"
                };
                Program.Main(args);
            });
            t.Start();
            threads.Add(t);

            // start crawler 
            t = new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "localhost:36002",
                "-t",
                "c",
                "-p",
                "localhost:36000",
                "-z",
                "localhost:2181"
                };
                Program.Main(args);
            });
            t.Start();
            threads.Add(t);

            //extracter proxy
            t = new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "localhost:37000",
                "-t",
                "ep",
                "-z",
                "localhost:2181"
                };
                Program.Main(args);
            });
            t.Start();
            threads.Add(t);

            //extracter
            t = new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "localhost:37001",
                "-t",
                "e",
                "-p",
                "localhost:37000",
                "-z",
                "localhost:2181"
                };
                Program.Main(args);
            });
            t.Start();
            threads.Add(t);

            //extracter
            t = new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "localhost:37002",
                "-t",
                "e",
                "-p",
                "localhost:37000",
                "-z",
                "localhost:2181"
                };
                Program.Main(args);
            });
            t.Start();
            threads.Add(t);
        }
    }
}
