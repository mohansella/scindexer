using SCIndexer.Journal;
using SCIndexer.Record;
using System;

namespace SCIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            var scanFolder = @"D:\test";
            var journalScanner = new SimpleJournalScanner(scanFolder);
            var journalListener = new ConsoleFileListener();
            journalScanner.Scan(journalListener);
        }
    }
}
