using SCIndexer.Journal;
using SCIndexer.Record;
using SCIndexer.Record.Listener;
using System;

namespace SCIndexer
{
    class Program
    {

        static void Main(string[] args)
        {
            var scanFolder = @"D:\test";
            var listener = new ConsoleFileListener();
            //var scanner = new SimpleJournalScanner(scanFolder, listener);
            var scanner = new SimpleFileRecordScanner(scanFolder, listener);
            scanner.Scan();
        }

    }
}
