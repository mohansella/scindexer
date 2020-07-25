using DeviceIOControlLib.Objects.Usn;
using DeviceIOControlLib.Wrapper;
using Microsoft.Win32.SafeHandles;
using SCIndexer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private SafeFileHandle hddHandle;
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
            this.hddHandle = NativeWrapper.CreateFile(drivePath, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);

            if (hddHandle.IsInvalid)
            {
                var lastError = Marshal.GetLastWin32Error();
                var hintMessage = lastError == 5 ? ". Try Run As Administrator" : string.Empty;
                throw new Win32Exception(lastError, $"{new Win32Exception(lastError).Message} for path: \"{drivePath}\" {hintMessage}");
            }

            this.usnIo = new UsnDeviceWrapper(hddHandle, true);
        }


        protected bool AcceptFile(string fileFolder, string fileFullName, string fileExtension)
        {
            return true;
        }

        public void Scan(JournalScanListener listener)
        {
            USN firstUsn = this.fromUsn;

            do
            {
                var usnRecords = this.usnIo.FileSystemReadUsnJournal(UsnJournalReasonMask.All, firstUsn);
                foreach (USN_RECORD_V2 usnRecord in usnRecords)
                {
                    
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
