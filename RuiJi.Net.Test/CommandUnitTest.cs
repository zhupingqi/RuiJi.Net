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
                "192.168.31.196:38000",
                "-t",
                "cp",
                "-z",
                "192.168.31.196:2181"
                };
                Program.Main(args);
            }).Start();


            // start crawler
            new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "192.168.31.196:39001",
                "-t",
                "c",
                "-p",
                "192.168.31.196:38000",
                "-z",
                "192.168.31.196:2181"
                };
                Program.Main(args);
            }).Start();

            // start crawler 
            new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "192.168.31.196:39002",
                "-t",
                "c",
                "-p",
                "192.168.31.196:38000",
                "-z",
                "192.168.31.196:2181"
                };
                Program.Main(args);
            }).Start();

            //extracter proxy
            new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "192.168.31.196:36000",
                "-t",
                "ep",
                "-z",
                "192.168.31.196:2181"
                };
                Program.Main(args);
            }).Start();

            //extracter
            new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "192.168.31.196:37001",
                "-t",
                "e",
                "-p",
                "192.168.31.196:36000",
                "-z",
                "192.168.31.196:2181"
                };
                Program.Main(args);
            }).Start();

            //extracter
            new Thread(() =>
            {
                var args = new string[] {
                "start",
                "-u",
                "192.168.31.196:37002",
                "-t",
                "e",
                "-p",
                "192.168.31.196:36000",
                "-z",
                "192.168.31.196:2181"
                };
                Program.Main(args);
            }).Start();

            Thread.Sleep(600000);
            Assert.IsTrue(true);
        }
    }
}
