using BetterWin32Errors;
using DeviceIOControlLib.Objects.Usn;
using DeviceIOControlLib.Wrapper;
using Microsoft.Win32.SafeHandles;
using SCIndexer.Native;
using SCIndexer.Record;
using SCIndexer.Record.Filter;
using SCIndexer.Record.Listener;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SCIndexer.Journal
{
    public class AbstractJournalScanner : IFileRecordScanner
    {


        protected readonly string scanFolder;
        protected readonly char driveLetter;
        protected readonly IFileRecordListener recordListener;
        protected readonly IFileRecordFilter recordFilter;
        protected readonly SafeFileHandle volumeHandle;
        protected readonly UsnDeviceWrapper usnIo;

        private readonly static string DriveLetterRegexString = @"^(\S)\:\\(.*)";

        public long FromUsn { get; set; }
        public long ScanLastUsn { get; protected set; }
          
        protected AbstractJournalScanner(string scanFolder, IFileRecordListener recordListener, IFileRecordFilter recordFilter = null)
        {
            this.scanFolder = Path.GetFullPath(scanFolder);
            this.recordFilter = recordFilter;
            this.recordListener = recordListener;
            var driveLetterRegex = new Regex(DriveLetterRegexString);
            var driveLetterMatch = driveLetterRegex.Match(this.scanFolder);
            if(driveLetterMatch.Success)
            {
                this.driveLetter = driveLetterMatch.Groups.Values.ElementAt(1).Value.ToUpper()[0]; //extract first letter in upper case
            }
            else
            {
                throw new ArgumentException($"invalid scanFolder path: {this.scanFolder}");
            }

            var drivePath = $"\\\\.\\{driveLetter}:";
            this.volumeHandle = Kernel32.CreateFile(drivePath, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);

            if (volumeHandle.IsInvalid)
            {
                var lastError = Win32Exception.GetLastWin32Error();
                var hintMessage = lastError == Win32Error.ERROR_ACCESS_DENIED ? ". Try Run As Administrator" : string.Empty;
                throw new Win32Exception(lastError, $"{new Win32Exception(lastError).Message} for path: \"{drivePath}\" {hintMessage}");
            }

            this.usnIo = new UsnDeviceWrapper(volumeHandle, true);
        }

        protected virtual bool AcceptFile(string filePath, string fileName)
        {
            return true;
        }

        protected virtual bool Filter(string filePath, FileRecordReason reason)
        {
            return this.recordFilter == null || this.recordFilter.Filter(filePath, reason);
        }

        public void Scan()
        {
            USN firstUsn = this.FromUsn;

            do
            {
                var usnRecords = this.usnIo.FileSystemReadUsnJournal(UsnJournalReasonMask.All, firstUsn);
                foreach (USN_RECORD_V2 usnRecord in usnRecords)
                {
                    var fileDescriptor = new FILE_ID_DESCRIPTOR
                    {
                        dwSize = 100,
                        FileReferenceNumber = usnRecord.FileReferenceNumber,
                        type = FILE_ID_TYPE.FileIdType
                    };
                    var fileHandle = Kernel32.OpenFileById(this.volumeHandle, ref fileDescriptor, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileAttributes.Normal);
                    
                    if(fileHandle.IsInvalid)
                    {
                        continue;
                    } 

                    var filePath = Kernel32.GetFinalPathNameByHandle(fileHandle);
                    var reason = Convert(usnRecord.Reason);
                    if (this.AcceptFile(filePath, usnRecord.FileName) && this.Filter(filePath, reason))
                    {
                        this.recordListener.Listen(filePath, reason);
                    }
                }
                
                if(usnRecords.Length == 0)
                {
                    break;
                }

                firstUsn.Usn = usnRecords.Last().Usn.Usn + 1;
            } while (true);

            this.ScanLastUsn = firstUsn.Usn;
        }
 
        public static FileRecordReason Convert(UsnJournalReasonMask mask)
        {
            switch(mask)
            {
                case UsnJournalReasonMask.USN_REASON_FILE_CREATE:
                    return FileRecordReason.Create;
                case UsnJournalReasonMask.USN_REASON_FILE_DELETE:
                    return FileRecordReason.Delete;
                default:
                    return FileRecordReason.Update;
            }
        }
    }

}
