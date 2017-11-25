using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPatternMatching
{
    class LineEntry
    {
        private DateTime timestamp;
        private Int32 stage;

        public LineEntry()
        {

        }

        public LineEntry(DateTime timestamp, Int32 stage)
        {
            this.timestamp = timestamp;
            this.stage = stage;
        }

        public DateTime Timestamp
        {
            get
            {
                return this.timestamp;
            }
        }
        public Int32 Stage
        {
            get
            {
                return this.stage;
            }
        }

    }
}
