using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Journal
{
    class ConsoleJournalScanListener : JournalScanListener
    {
        public void Accept(string fileFolderPth, string fileFullName)
        {
            Console.WriteLine($"file: {fileFolderPth}\\{fileFullName}");
        }
    }
}
