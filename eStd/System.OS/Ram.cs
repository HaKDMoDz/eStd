﻿using System;
using System.Runtime.InteropServices;

namespace SystemInfoLib.Windows
{


    /// <summary>
    /// Class for getting information related to RAM
    /// </summary>
    public sealed class Ram
    {
        #region Properties

        /// <summary>
        /// Gets the total physical memory in bytes
        /// </summary>
        public static string TotalPhysicalMemory
        {
            get
            {
                MemoryStatusEx mEx = new MemoryStatusEx();
                if (GlobalMemoryStatusEx(mEx))
                {
                    return Convert.ToString(mEx.ullTotalPhys);
                }

                return "";
            }
        }

        /// <summary>
        /// Gets the available physical memory in bytes
        /// </summary>
        public static string AvailablePhysicalMemory
        {
            get
            {
                MemoryStatusEx mEx = new MemoryStatusEx();
                if (GlobalMemoryStatusEx(mEx))
                {
                    return Convert.ToString(mEx.ullAvailPhys);
                }

                return "";
            }
        }

        /// <summary>
        /// Gets the current committed memory limit for the system or the 
        /// current process, whichever is smaller, in bytes.
        /// </summary>
        public static string TotalPageFile
        {
            get
            {
                MemoryStatusEx mEx = new MemoryStatusEx();
                if (GlobalMemoryStatusEx(mEx))
                {
                    return Convert.ToString(mEx.ullTotalPageFile);
                }

                return "";
            }
        }

        /// <summary>
        /// Gets the maximum amount of memory the current process can commit, in bytes
        /// </summary>
        public static string AvailablePageFile
        {
            get
            {
                MemoryStatusEx mEx = new MemoryStatusEx();
                if (GlobalMemoryStatusEx(mEx))
                {
                    return Convert.ToString(mEx.ullAvailPageFile);
                }

                return "";
            }
        }

        /// <summary>
        /// Gets the size of the user-mode portion of the virtual 
        /// address space of the calling process, in bytes. 
        /// </summary>
        public static string TotalVirtual
        {
            get
            {
                MemoryStatusEx mEx = new MemoryStatusEx();
                if (GlobalMemoryStatusEx(mEx))
                {
                    return Convert.ToString(mEx.ullTotalVirtual);
                }

                return "";
            }
        }

        /// <summary>
        /// Gets the amount of unreserved and uncommitted memory currently in the user-mode 
        /// portion of the virtual address space of the calling process, in bytes.
        /// </summary>
        public static string AvailableVirtual
        {
            get
            {
                MemoryStatusEx mEx = new MemoryStatusEx();
                if (GlobalMemoryStatusEx(mEx))
                {
                    return Convert.ToString(mEx.ullAvailVirtual);
                }

                return "";
            }
        }


        #endregion

        #region P/Invoke Related

        /// <summary>
        /// Class that represents the C/C++ structure MEMORYSTATUSEX
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class MemoryStatusEx
        {
            /// <summary>
            /// The size of the structure, in bytes.
            /// </summary>
            internal uint dwLength;

            /// <summary>
            /// A number between 0 and 100 that specifies the approximate percentage of physical memory 
            /// that is in use (0 indicates no memory use and 100 indicates full memory use).
            /// </summary>
            internal uint dwMemoryLoad;

            /// <summary>
            /// The amount of actual physical memory, in bytes.
            /// </summary>
            internal ulong ullTotalPhys;

            /// <summary>
            /// The amount of physical memory currently available, in bytes.
            /// 
            /// This is the amount of physical memory that can be immediately 
            /// reused without having to write its contents to disk first. 
            /// 
            /// It is the sum of the size of the standby, free, and zero lists.
            /// </summary>
            internal ulong ullAvailPhys;

            /// <summary>
            /// The current committed memory limit for the system or the 
            /// current process, whichever is smaller, in bytes.
            /// </summary>
            internal ulong ullTotalPageFile;

            /// <summary>
            /// The maximum amount of memory the current process can commit, in bytes.
            /// </summary>
            internal ulong ullAvailPageFile;

            /// <summary>
            /// The size of the user-mode portion of the virtual 
            /// address space of the calling process, in bytes. 
            /// </summary>
            internal ulong ullTotalVirtual;

            /// <summary>
            /// The amount of unreserved and uncommitted memory currently in the user-mode 
            /// portion of the virtual address space of the calling process, in bytes.
            /// </summary>
            internal ulong ullAvailVirtual;

            /// <summary>
            /// Reserved. This value is always 0.
            /// </summary>
            internal ulong ullAvailExtendedVirtual;

            /// <summary>
            /// Constructor
            /// </summary>
            internal MemoryStatusEx()
            {
                this.dwLength = (uint) Marshal.SizeOf(this);
            }
        }


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatusEx lpBuffer);


        #endregion
    }
}
