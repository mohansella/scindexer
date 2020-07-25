using System;
using System.Collections.Generic;
using System.Text;

namespace SCIndexer.Record
{
    public interface IFileRecordListener
    {
        public void Listen(string filePath, FileRecordReason reason);
    }
}
