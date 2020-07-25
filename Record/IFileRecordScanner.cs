using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Record
{
    interface IFileRecordScanner
    {
        public void Scan(IFileRecordListener listener);
    }
}
