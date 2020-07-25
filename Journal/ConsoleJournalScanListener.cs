using DeviceIOControlLib.Objects.Usn;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Journal
{
    class ConsoleJournalScanListener : JournalScanListener
    {
        public void Listen(string filePath, string fileName, UsnJournalReasonMask reasonMask)
        {
            Console.WriteLine($"fileName: {fileName} path: {filePath}");
        }
    }
}
