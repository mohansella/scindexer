using SCIndexer.Record.Filter;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Record.Listener
{
    class ConsoleFileListener : IFileRecordListener
    {

        public ConsoleFileListener()
        {

        }


        public void Listen(string filePath, FileRecordReason reason)
        {
            Console.WriteLine($"filePath: {filePath} reason: {reason}");
        }
    }
}
