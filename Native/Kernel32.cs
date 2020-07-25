using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;


namespace SCIndexer.Native
{
    public class Kernel32
    {

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(string lpFileName,
           [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
           [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
           IntPtr lpSecurityAttributes,
           [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
           [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
           IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle OpenFileById(
            SafeFileHandle volumeHandle, 
            ref FILE_ID_DESCRIPTOR lpFileId,
           [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
           [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
            IntPtr lpSecurityAttributes,
           [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetFileInformationByHandle(SafeFileHandle fileHandle, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetFinalPathNameByHandle(IntPtr fileHandle, [In, Out] StringBuilder path, int bufLen, int flags);


        public static string GetFinalPathNameByHandle(SafeFileHandle fileHandle)
        {
            StringBuilder path = new StringBuilder(10);
            var length = GetFinalPathNameByHandle(fileHandle.DangerousGetHandle(), path, path.Capacity, 0);
            
            if(length != 0 && length + 1 > path.Capacity) //low memory. note: the length is in bytes, and path.Capacity is length of wchar.
            {
                path.Capacity = length + 1;
                length = GetFinalPathNameByHandle(fileHandle.DangerousGetHandle(), path, path.Capacity, 0);
            }

            if(length == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else
            {
                return path.ToString();
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct FILE_ID_DESCRIPTOR
    {
        [FieldOffset(0)] public uint dwSize;
        [FieldOffset(4)] public FILE_ID_TYPE type;
        // [FieldOffset(8)] public Guid guid;
        [FieldOffset(8)] public ulong FileReferenceNumber;
    }

    public enum FILE_ID_TYPE
    {
        FileIdType = 0,
        ObjectIdType = 1,
        ExtendedFileIdType = 2,
        MaximumFileIdType
    };

    public struct BY_HANDLE_FILE_INFORMATION
    {
        public uint FileAttributes;
        public FILETIME CreationTime;
        public FILETIME LastAccessTime;
        public FILETIME LastWriteTime;
        public uint VolumeSerialNumber;
        public uint FileSizeHigh;
        public uint FileSizeLow;
        public uint NumberOfLinks;
        public uint FileIndexHigh;
        public uint FileIndexLow;
    }

}
