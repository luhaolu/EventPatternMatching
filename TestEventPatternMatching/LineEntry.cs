using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEventPatternMatching
{
    class LineEntry
    {

        private string timestamp;
        private string stage;

        public LineEntry(string timestamp, string stage)
        {
            this.timestamp = timestamp;
            this.stage = stage;
        }

        public string Timestamp
        {
            get
            {
                return this.timestamp;
            }
        }
        public string Stage
        {
            get
            {
                return this.stage;
            }
        }

        public static string GetCSVFile(List<LineEntry> list)
        {
            String contents = "Timestamp\tValue\n";
            foreach(LineEntry entry in list)
            {
                contents += entry.timestamp + "\t" + entry.stage + "\n";
            }
            return contents;
        }

    }
}
