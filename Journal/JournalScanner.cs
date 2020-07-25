using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Journal
{
    public interface JournalScanner
    {

        public long Scan(JournalScanListener listener);

    }
}
