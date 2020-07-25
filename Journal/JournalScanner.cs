using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Journal
{
    public interface JournalScanner
    {

        public void Scan(JournalScanListener listener);

    }
}
