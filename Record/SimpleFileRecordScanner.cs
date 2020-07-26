using SCIndexer.Record.Filter;
using SCIndexer.Record.Listener;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SCIndexer.Record
{
    public class SimpleFileRecordScanner : IFileRecordScanner
    {

        protected readonly string scanFolder;
        protected readonly IFileRecordListener recordListener;
        protected readonly IFileRecordFilter recordFilter;

        public SimpleFileRecordScanner(string scanFolder, IFileRecordListener recordListener, IFileRecordFilter recordFilter = null)
        {
            this.scanFolder = scanFolder;
            this.recordListener = recordListener;
            this.recordFilter = recordFilter;
        }

        public void Scan()
        {
            this.Scan(new DirectoryInfo(this.scanFolder));
        }

        private void Scan(DirectoryInfo directoryInfo)
        {
            this.recordListener.Listen(directoryInfo.FullName, FileRecordReason.Create);
            foreach (var fileInfo in directoryInfo.EnumerateFiles())
            {
                this.Scan(fileInfo);
            }
            foreach(var subDirectory in directoryInfo.EnumerateDirectories())
            {
                this.Scan(subDirectory);
            }
        }

        private void Scan(FileInfo fileInfo)
        {
            this.recordListener.Listen(fileInfo.FullName, FileRecordReason.Create);
        }

    }
}
