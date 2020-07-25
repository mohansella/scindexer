using DeviceIOControlLib.Objects.Usn;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Journal
{
    public interface JournalScanListener
    {

        public void Listen(string filePath, string fileName, UsnJournalReasonMask reasonMask);

    }
}
