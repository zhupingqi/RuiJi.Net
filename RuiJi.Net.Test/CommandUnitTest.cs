using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuiJi.Cmd;

namespace RuiJi.Net.Test
{
    [TestClass]
    public class CommandUnitTest
    {
        [TestMethod]
        public void TestCmd()
        {
            // start crawler proxy
            new Thread(() =>
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
            }).Start();

            // start crawler
            new Thread(() =>
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
            }).Start();

            // start crawler 
            new Thread(() =>
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
            }).Start();

            //extracter proxy
            new Thread(() =>
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
            }).Start();

            //extracter
            new Thread(() =>
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
            }).Start();

            //extracter
            new Thread(() =>
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
            }).Start();

            Thread.Sleep(600000);
            Assert.IsTrue(true);
        }
    }
}
