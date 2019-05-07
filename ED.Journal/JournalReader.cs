using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ED.Journal
{
    public class JournalReader
    {
        private readonly JournalReadMode _readMode;

        public enum JournalReadMode
        {
            Indexed,
            FullRead
        }

        public JournalReader(JournalReadMode readMode = JournalReadMode.Indexed)
        {
            _readMode = readMode;

        }

        public async Task<List<Journal>> ReadJournalsAsync(string path, string filenamePattern)
        {
            var journals = new List<Journal>();

            var files = Directory.GetFiles(path, filenamePattern);
            foreach (var file in files)
            {
                Journal journal = await LoadJournalFile(file);
                journals.Add(journal);
            }

            return journals;
        }

        public Task<Journal> LoadJournalFile(string file)
        {
            var journal = new Journal();
            // Set the FileDateTime property from the date part of the filename

            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                var journalEvent = new JournalEvent();

                // Read each line as a json object
                if (JsonConvert.DeserializeObject(line) is JObject jObj)
                {
                    // Parse out the known field types where appropriate as top level Journal and Event Entries
                    var dateTimeString = jObj["timestamp"].ToString();
                    journalEvent.DateTime = DateTime.Parse(dateTimeString);

                    var eventString = jObj["event"].ToString();
                    journalEvent.EventType = eventString;

                    if (eventString == "FileHeader")
                    {
                        var gameVersion = jObj["gameversion"];
                        if (gameVersion != null)
                            journal.GameVersion = gameVersion.ToString();
                        
                        var build = jObj["build"];
                        if (build != null)
                            journal.Build = build.ToString();

                    }

                    if (eventString == "LoadGame")
                    {
                        var commanderName = jObj["Commander"];
                        if (commanderName != null)
                        {
                            journal.CommanderName = commanderName.ToString();
                        }

                    }


                    // Add every field from the Event
                    var propertyNames = jObj.Children().Select(x => x.Path);
                    foreach (var name in propertyNames)
                    {
                        var val = jObj[name]?.ToString();
                        journalEvent.Entries.Add(name, val);
                    }


                    journal.Events.Add(journalEvent);

                }
                else
                {
                    // Log lines to an error file where a Json Object wasn't able to be cast
                }

            }

            return Task.FromResult(journal);
        }
    }
}
