using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Record
{
    class ConsoleFileListener : IFileRecordListener
    {
        public void Listen(string filePath, FileRecordReason reason)
        {
            Console.WriteLine($"filePath: {filePath} reason: {reason}");
        }
    }
}
