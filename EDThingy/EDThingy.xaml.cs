﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ED.Journal;

namespace EDThingy
{
    /// <summary>
    /// Interaction logic for EDThingy.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, List<Journal>> commanderJournals = new Dictionary<string, List<Journal>>();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void ReadJournals_Click(object sender, RoutedEventArgs e)
        {
            ReadJournalsAsync();

        }


        private async Task ReadJournalsAsync()
        {
            var journalPath = ConfigurationManager.AppSettings["JournalPath"];
            var filenamePattern = ConfigurationManager.AppSettings["FilenamePattern"];
            var reader = new JournalReader(JournalReader.JournalReadMode.FullRead);
            await reader.ReadJournalsAsync(journalPath, filenamePattern);
            int a = 1;
            //if (reader.Journals.Any())
            //{
            //    CommanderNames.ItemsSource = null;
            //    CommanderNames.SelectedIndex = -1;
            //    EventsList.ItemsSource = null;
            //    commanderJournals.Clear();

            //    var cmdrNames = await reader.CommanderNamesAsync();

            //    foreach (var commander in cmdrNames)
            //    {
            //        var cmdrJournals = reader.Journals.Where(x => x.CommanderName == commander).ToList();
            //        if (cmdrJournals.Any())
            //        {
            //            commanderJournals.Add(commander, cmdrJournals);
            //        }
            //    }

            //    CommanderNames.ItemsSource = cmdrNames;
            //    CommanderNames.SelectedIndex = 0;

            //}


        }


        private void CommanderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetEventListView();

        }

        private void SetEventListView(params string[] eventTypeFilters)
        {
            if (CommanderNames.SelectedValue == null)
                return;

            var items = new List<EventsGridItem>();
            var cmdrJournals = commanderJournals[CommanderNames.SelectedValue.ToString()];
            foreach (var journal in cmdrJournals)
            {
                
                foreach (var journalEvent in journal.Events)
                {
                    var toAddOrNotToAdd = true;
                    if (eventTypeFilters.Length > 0 && !eventTypeFilters.Contains(journalEvent.EventType))
                        toAddOrNotToAdd = false;

                    if(toAddOrNotToAdd)
                    {
                        items.Add(new EventsGridItem
                        {
                            DateTime = journalEvent.DateTime,
                            EventType = journalEvent.EventType,
                            Fields = FormatFieldEntries(journalEvent.Entries)

                        });
                    }
                    
                }

            }

            EventsList.ItemsSource = items;
        }

        private string FormatFieldEntries(Dictionary<string, string> journalEventEntries)
        {
            var vals = new List<string>();
            foreach (var key in journalEventEntries.Keys)
            {
                if(!new[] { "timestamp", "event" }.Contains(key))
                    vals.Add($"{key}:{journalEventEntries[key]}");

            }

            return String.Join(" - ", vals);
        }

        private void FilterEventButtonClick(object sender, RoutedEventArgs e)
        {
            SetEventListView(FilterEventText.Text.Split(','));

            
        }
    }
}
