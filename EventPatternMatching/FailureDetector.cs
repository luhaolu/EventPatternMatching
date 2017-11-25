
using EventPatternMatching;
using RC.CodingChallenge;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace EventPatternMatching
{
    public class FailureDetector : IEventCounter
    {
        private static FailureDetector instance;

        private ConcurrentDictionary<string, int> eventHistory = new ConcurrentDictionary<string, int>();

        private FailureDetector() { }

        public static FailureDetector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FailureDetector();
                }
                return instance;
            }
        }

        public int GetEventCount(string deviceID)
        {
            int eventCount = 0;
            this.eventHistory.TryGetValue(deviceID, out eventCount);
            return eventCount;
        }

        public void ParseEvents(string deviceID, StreamReader eventLog)
        {
            LineEntry previous = new LineEntry();
            LineEntry current;
            Boolean sequenceMatch = false;

            while (!eventLog.EndOfStream)
            {
                // read a line from log
                String line = eventLog.ReadLine();
                string[] parsedLine = line.Split('\t');

                try
                {
                    current = new LineEntry(Convert.ToDateTime(parsedLine[0]), Int32.Parse(parsedLine[1]));
                }
                catch (Exception)
                {
                    // unable to convert from string, skip line
                    continue;
                }

                // if no stage changed, skip the line
                if (current.Stage == previous.Stage) { continue; }

                // if stage changed from 3 to 2, check for stage 3's duration
                else if (previous.Stage == 3 && current.Stage == 2)
                {
                    // check duration
                    TimeSpan diff = current.Timestamp - previous.Timestamp;
                    if (diff.TotalMinutes >= 5.0)
                    {
                        sequenceMatch = true;
                    }
                }

                else if (sequenceMatch && current.Stage == 0)
                {
                    // Failere event detected, update history and reset status
                    this.eventHistory.AddOrUpdate(deviceID, 1, (key, oldValue) => oldValue + 1);
                    sequenceMatch = false;
                }

                previous = current;
            }
        }
    }
}
