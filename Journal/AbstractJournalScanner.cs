﻿using BetterWin32Errors;
using DeviceIOControlLib.Objects.Usn;
using DeviceIOControlLib.Wrapper;
using Microsoft.Win32.SafeHandles;
using SCIndexer.Native;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SCIndexer.Journal
{
    public class AbstractJournalScanner : JournalScanner
    {

        private string scanFolder;
        private char driveLetter;
        private SafeFileHandle volumeHandle;
        private UsnDeviceWrapper usnIo;
        private long fromUsn;

        private readonly static string DriveLetterRegexString = @"^\S\:\\";

        protected AbstractJournalScanner(string scanFolder, long fromUsn = 0)
        {
            this.scanFolder = Path.GetFullPath(scanFolder);
            this.fromUsn = fromUsn;
            var driveLetterRegex = new Regex(DriveLetterRegexString);
            var driveLetterMatch = driveLetterRegex.Match(this.scanFolder);
            if(driveLetterMatch.Success)
            {
                this.driveLetter = driveLetterMatch.Groups.Values.ElementAt(0).Value.ToUpper()[0]; //extract first letter in upper case
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


        protected bool AcceptFile(string filePath, string fileName)
        {
            return true;
        }

        public void Scan(JournalScanListener listener)
        {
            USN firstUsn = this.fromUsn;

            do
            {
                var usnRecords = this.usnIo.FileSystemReadUsnJournal(UsnJournalReasonMask.All, firstUsn, 100);
                foreach (USN_RECORD_V2 usnRecord in usnRecords)
                {
                    var fileDescriptor = new FILE_ID_DESCRIPTOR();
                    fileDescriptor.dwSize = 100;
                    fileDescriptor.FileReferenceNumber = usnRecord.FileReferenceNumber;
                    fileDescriptor.type = FILE_ID_TYPE.FileIdType;
                    var fileHandle = Kernel32.OpenFileById(this.volumeHandle, ref fileDescriptor, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileAttributes.Normal);
                    
                    if(fileHandle.IsInvalid)
                    {
                        continue;
                    } 

                    var filePath = Kernel32.GetFinalPathNameByHandle(fileHandle);
                    if(this.AcceptFile(filePath, usnRecord.FileName))
                    {
                        listener.Listen(filePath, usnRecord.FileName, usnRecord.Reason);
                    }
                }
                
                if(usnRecords.Length == 0)
                {
                    break;
                }

                firstUsn.Usn = usnRecords.Last().Usn.Usn + 1;
            } while (true);


        }
    }
}
