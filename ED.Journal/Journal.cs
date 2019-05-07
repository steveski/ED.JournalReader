using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED.Journal
{
    public class Journal
    {
        public Journal()
        {
            Events = new List<JournalEvent>();
        }


        public DateTime FileDateTime { get; set; }
        public string GameVersion { get; set; }
        public string Build { get; set; }

        public string CommanderName { get; set; }

        public List<JournalEvent> Events { get; set; }


    }
}
