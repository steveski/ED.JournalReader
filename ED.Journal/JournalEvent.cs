using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ED.Journal
{
    public class JournalEvent
    {
        public JournalEvent()
        {
            Entries = new Dictionary<string, string>();
        }

        public DateTime DateTime { get; set; }
        public string EventType { get; set; }

        public Dictionary<string, string> Entries { get; set; }


    }
}