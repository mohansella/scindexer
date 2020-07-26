using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Record.Filter
{
    public interface IFileRecordFilter
    {
        public bool Filter(string filePath, FileRecordReason reason);
    }
}
