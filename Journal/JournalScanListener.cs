using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Journal
{
    public interface JournalScanListener
    {

        public void Accept(string fileFolderPth, string fileFullName);

    }
}
