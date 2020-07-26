using SCIndexer.Record.Filter;
using SCIndexer.Record.Listener;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Journal
{
    public class SimpleJournalScanner : AbstractJournalScanner
    {

        private string extendedContextPath;

        public SimpleJournalScanner(string scanFolder, IFileRecordListener recordListener, IFileRecordFilter recordFilter = null) 
            : base(scanFolder, recordListener, recordFilter)
        {
            this.extendedContextPath = $"\\\\?\\{scanFolder}";  //  sample path: \\?\D:\test\test.txt
        }

        protected override bool AcceptFile(string filePath, string fileName)
        {
            return filePath.StartsWith(this.extendedContextPath, StringComparison.OrdinalIgnoreCase);
        }

    }
}
