using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using EventPatternMatching;
using System.Text;
using System.Threading;

namespace TestEventPatternMatching
{
    [TestClass]
    public class FailureDetectorTest
    {
        [TestMethod]
        public void TestSingleFailuere()
        {
            List<LineEntry> list = new List<LineEntry>();
            list.Add(new LineEntry("2011-03-07 06:25:32", "2"));
            list.Add(new LineEntry("2011-03-07 09:15:55", "3"));
            list.Add(new LineEntry("2011-03-07 12:00:00", "3"));
            list.Add(new LineEntry("2011-03-07 12:03:27", "2"));
            list.Add(new LineEntry("2011-03-07 20:23:01", "0"));
                        
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(LineEntry.GetCSVFile(list));
            MemoryStream stream = new MemoryStream(byteArray);
            StreamReader reader = new StreamReader(stream);

            FailureDetector.Instance.ParseEvents("Device1", reader);
            Assert.AreEqual(1, FailureDetector.Instance.GetEventCount("Device1"));
        }

        [TestMethod]
        public void TestNoFailuere1()
        {
            // In stage 3 less than 5 minutes
            List<LineEntry> list = new List<LineEntry>();
            list.Add(new LineEntry("2011-03-07 06:25:32", "2"));
            list.Add(new LineEntry("2011-03-07 12:00:00", "3"));
            list.Add(new LineEntry("2011-03-07 12:00:01", "3"));
            list.Add(new LineEntry("2011-03-07 12:03:27", "2"));
            list.Add(new LineEntry("2011-03-07 20:23:01", "0"));
            
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(LineEntry.GetCSVFile(list));
            MemoryStream stream = new MemoryStream(byteArray);
            StreamReader reader = new StreamReader(stream);
            
            FailureDetector.Instance.ParseEvents("Device2", reader);
            Assert.AreEqual(0, FailureDetector.Instance.GetEventCount("Device2"));
        }

        [TestMethod]
        public void TestNoFailuere2()
        {
            // In stage 3 more than 5 minutes but didn't go to stage 2
            List<LineEntry> list = new List<LineEntry>();
            list.Add(new LineEntry("2011-03-07 06:25:32", "2"));
            list.Add(new LineEntry("2011-03-07 09:15:55", "3"));
            list.Add(new LineEntry("2011-03-07 12:00:01", "3"));
            list.Add(new LineEntry("2011-03-07 12:03:27", "1"));
            list.Add(new LineEntry("2011-03-07 12:05:27", "1"));
            list.Add(new LineEntry("2011-03-07 20:23:01", "0"));
            
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(LineEntry.GetCSVFile(list));
            MemoryStream stream = new MemoryStream(byteArray);
            StreamReader reader = new StreamReader(stream);
            
            FailureDetector.Instance.ParseEvents("Device3", reader);
            Assert.AreEqual(0, FailureDetector.Instance.GetEventCount("Device3"));
        }

        [TestMethod]
        public void TestMultipleFailuere()
        {
            List<LineEntry> list = new List<LineEntry>();
            list.Add(new LineEntry("2011-03-07 06:25:32", "2"));
            list.Add(new LineEntry("2011-03-07 09:15:55", "3"));
            list.Add(new LineEntry("2011-03-07 12:00:00", "3"));
            list.Add(new LineEntry("2011-03-07 12:03:27", "2"));
            list.Add(new LineEntry("2011-03-07 20:23:01", "0"));

            list.Add(new LineEntry("2011-03-08 06:25:32", "1"));
            list.Add(new LineEntry("2011-03-08 06:25:32", "1"));
            list.Add(new LineEntry("2011-03-09 09:15:55", "2"));
            list.Add(new LineEntry("2011-03-10 09:15:55", "2"));
            list.Add(new LineEntry("2011-03-11 12:00:00", "3"));
            list.Add(new LineEntry("2011-03-12 12:03:27", "2"));
            list.Add(new LineEntry("2011-03-13 12:03:27", "2"));
            list.Add(new LineEntry("2011-03-14 12:03:27", "3"));
            list.Add(new LineEntry("2011-03-15 12:03:27", "3"));
            list.Add(new LineEntry("2011-03-16 20:23:01", "0"));

            list.Add(new LineEntry("2011-03-17 09:15:55", "2"));
            list.Add(new LineEntry("2011-03-18 12:00:00", "3"));
            list.Add(new LineEntry("2011-03-19 12:00:00", "3"));
            list.Add(new LineEntry("2011-03-20 12:00:00", "1"));
            list.Add(new LineEntry("2011-03-21 12:00:00", "3"));
            list.Add(new LineEntry("2011-03-22 12:00:00", "2"));
            list.Add(new LineEntry("2011-03-23 20:23:01", "0"));
            
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(LineEntry.GetCSVFile(list));
            MemoryStream stream = new MemoryStream(byteArray);
            StreamReader reader = new StreamReader(stream);
            
            FailureDetector.Instance.ParseEvents("Device4", reader);
            Assert.AreEqual(3, FailureDetector.Instance.GetEventCount("Device4"));
        }

        [TestMethod]
        public void TestMultipleFailuereWithMultiThread()
        {
            // Contain 3 failures
            List<LineEntry> list1 = new List<LineEntry>();
            list1.Add(new LineEntry("2011-03-07 06:25:32", "2"));
            list1.Add(new LineEntry("2011-03-07 09:15:55", "3"));
            list1.Add(new LineEntry("2011-03-07 12:00:00", "3"));
            list1.Add(new LineEntry("2011-03-07 12:03:27", "2"));
            list1.Add(new LineEntry("2011-03-07 20:23:01", "0"));

            list1.Add(new LineEntry("2011-03-08 06:25:32", "1"));
            list1.Add(new LineEntry("2011-03-08 06:25:32", "1"));
            list1.Add(new LineEntry("2011-03-09 09:15:55", "2"));
            list1.Add(new LineEntry("2011-03-10 09:15:55", "2"));
            list1.Add(new LineEntry("2011-03-11 12:00:00", "3"));
            list1.Add(new LineEntry("2011-03-12 12:03:27", "2"));
            list1.Add(new LineEntry("2011-03-13 12:03:27", "2"));
            list1.Add(new LineEntry("2011-03-14 12:03:27", "3"));
            list1.Add(new LineEntry("2011-03-15 12:03:27", "3"));
            list1.Add(new LineEntry("2011-03-16 20:23:01", "0"));

            list1.Add(new LineEntry("2011-03-17 09:15:55", "2"));
            list1.Add(new LineEntry("2011-03-18 12:00:00", "3"));
            list1.Add(new LineEntry("2011-03-19 12:00:00", "3"));
            list1.Add(new LineEntry("2011-03-20 12:00:00", "1"));
            list1.Add(new LineEntry("2011-03-21 12:00:00", "3"));
            list1.Add(new LineEntry("2011-03-22 12:00:00", "2"));
            list1.Add(new LineEntry("2011-03-23 20:23:01", "0"));

            // Contain 2 failures
            List<LineEntry> list2 = new List<LineEntry>();
            list2.Add(new LineEntry("2011-03-07 06:25:32", "2"));
            list2.Add(new LineEntry("2011-03-07 09:15:55", "3"));
            list2.Add(new LineEntry("2011-03-07 12:00:00", "3"));
            list2.Add(new LineEntry("2011-03-07 12:03:27", "2"));
            list2.Add(new LineEntry("2011-03-07 20:23:01", "0"));

            list2.Add(new LineEntry("2011-03-08 06:25:32", "1"));
            list2.Add(new LineEntry("2011-03-08 06:25:32", "1"));
            list2.Add(new LineEntry("2011-03-09 09:15:55", "2"));
            list2.Add(new LineEntry("2011-03-10 09:15:55", "2"));
            list2.Add(new LineEntry("2011-03-11 12:00:00", "3"));
            list2.Add(new LineEntry("2011-03-12 12:03:27", "2"));
            list2.Add(new LineEntry("2011-03-13 12:03:27", "2"));
            list2.Add(new LineEntry("2011-03-14 12:03:27", "3"));
            list2.Add(new LineEntry("2011-03-15 12:03:27", "3"));
            list2.Add(new LineEntry("2011-03-16 20:23:01", "0"));

            // Contain 1 failures
            List<LineEntry> list3 = new List<LineEntry>();
            list3.Add(new LineEntry("2011-03-17 09:15:55", "2"));
            list3.Add(new LineEntry("2011-03-18 12:00:00", "3"));
            list3.Add(new LineEntry("2011-03-19 12:00:00", "3"));
            list3.Add(new LineEntry("2011-03-20 12:00:00", "1"));
            list3.Add(new LineEntry("2011-03-21 12:00:00", "3"));
            list3.Add(new LineEntry("2011-03-22 12:00:00", "2"));
            list3.Add(new LineEntry("2011-03-23 20:23:01", "0"));
            
            // convert string to stream
            StreamReader reader1 = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(LineEntry.GetCSVFile(list1))));
            StreamReader reader2 = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(LineEntry.GetCSVFile(list2))));
            StreamReader reader3 = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(LineEntry.GetCSVFile(list3))));

            Thread thread1 = new Thread(() => FailureDetector.Instance.ParseEvents("Device5", reader1));
            Thread thread2 = new Thread(() => FailureDetector.Instance.ParseEvents("Device5", reader2));
            Thread thread3 = new Thread(() => FailureDetector.Instance.ParseEvents("Device5", reader3));

            thread1.Start();
            thread2.Start();
            thread3.Start();

            Thread.Sleep(100);

            Assert.AreEqual(6, FailureDetector.Instance.GetEventCount("Device5"));
        }
    }
}
