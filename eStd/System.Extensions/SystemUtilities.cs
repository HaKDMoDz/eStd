using System;
using System.Runtime.InteropServices;

namespace Creek.Extensions
{
    /// <summary>
    /// Contains various operating system-related tools.
    /// </summary>
    public class SystemUtilities
    {
        #region Private Constants

        /// <summary>
        /// Version - NT Workstation
        /// </summary>
        private const int VERNTWORKSTATION = 1;

        /// <summary>
        /// Version - Domain Controller
        /// </summary>
        private const int VERNTDOMAINCONTROLLER = 2;

        /// <summary>
        /// Version - NT Server
        /// </summary>
        private const int VERNTSERVER = 3;

        /// <summary>
        /// Version - Small Business Suite
        /// </summary>
        private const int VERSUITESMALLBUSINESS = 1;

        /// <summary>
        /// Version - Enterprise suite.
        /// </summary>
        private const int VERSUITEENTERPRISE = 2;

        /// <summary>
        /// Version - Terminal suite
        /// </summary>
        private const int VERSUITETERMINAL = 16;

        /// <summary>
        /// Version - Data Center Suite
        /// </summary>
        private const int VERSUITEDATACENTER = 128;

        /// <summary>
        /// Version - Single User TS Suite
        /// </summary>
        private const int VERSUITESINGLEUSERTS = 256;

        /// <summary>
        /// Version - Personal Suite
        /// </summary>
        private const int VERSUITEPERSONAL = 512;

        /// <summary>
        /// Version - Blade Suite
        /// </summary>
        private const int VERSUITEBLADE = 1024;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the full version of the operating system running on this computer.
        /// </summary>
        public static string OSVersion
        {
            get
            {
                return System.Environment.OSVersion.Version.ToString();
            }
        }

        /// <summary>
        /// Gets the major version of the operating system running on this computer.
        /// </summary>
        public static int OSMajorVersion
        {
            get
            {
                return System.Environment.OSVersion.Version.Major;
            }
        }

        /// <summary>
        /// Gets the minor version of the operating system running on this computer.
        /// </summary>
        public static int OSMinorVersion
        {
            get
            {
                return System.Environment.OSVersion.Version.Minor;
            }
        }

        /// <summary>
        /// Gets the build version of the operating system running on this computer.
        /// </summary>
        public static int OSBuildVersion
        {
            get
            {
                return System.Environment.OSVersion.Version.Build;
            }
        }

        /// <summary>
        /// Gets the revision version of the operating system running on this computer.
        /// </summary>
        public static int OSRevisionVersion
        {
            get
            {
                return System.Environment.OSVersion.Version.Revision;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the product type of the operating system running on this computer.
        /// </summary>
        /// <returns>A string containing the the operating system product type.</returns>
        public static string GetOSProductType()
        {
            OSVERSIONINFOEX versionInfo = new OSVERSIONINFOEX();
            OperatingSystem info = System.Environment.OSVersion;

            versionInfo.VersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

            if (!GetVersionEx(ref versionInfo))
            {
                return string.Empty;
            }
            else
            {
                if (info.Version.Major == 4)
                {
                    if (versionInfo.ProductType == VERNTWORKSTATION)
                    {
                        // Windows NT 4.0 Workstation
                        return " Workstation";
                    }
                    else if (versionInfo.ProductType == VERNTSERVER)
                    {
                        // Windows NT 4.0 Server
                        return " Server";
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else if (info.Version.Major == 5)
                {
                    if (versionInfo.ProductType == VERNTWORKSTATION)
                    {
                        if ((versionInfo.SuiteMask & VERSUITEPERSONAL) == VERSUITEPERSONAL)
                        {
                            // Windows XP Home Edition
                            return " Home Edition";
                        }
                        else
                        {
                            // Windows XP / Windows 2000 Professional
                            return " Professional";
                        }
                    }
                    else if (versionInfo.ProductType == VERNTSERVER)
                    {
                        if (info.Version.Minor == 0)
                        {
                            if ((versionInfo.SuiteMask & VERSUITEDATACENTER) == VERSUITEDATACENTER)
                            {
                                // Windows 2000 Datacenter Server
                                return " Datacenter Server";
                            }
                            else if ((versionInfo.SuiteMask & VERSUITEENTERPRISE) == VERSUITEENTERPRISE)
                            {
                                // Windows 2000 Advanced Server
                                return " Advanced Server";
                            }
                            else
                            {
                                // Windows 2000 Server
                                return " Server";
                            }
                        }
                        else
                        {
                            if ((versionInfo.SuiteMask & VERSUITEDATACENTER) == VERSUITEDATACENTER)
                            {
                                // Windows Server 2003 Datacenter Edition
                                return " Datacenter Edition";
                            }
                            else if ((versionInfo.SuiteMask & VERSUITEENTERPRISE) == VERSUITEENTERPRISE)
                            {
                                // Windows Server 2003 Enterprise Edition
                                return " Enterprise Edition";
                            }
                            else if ((versionInfo.SuiteMask & VERSUITEBLADE) == VERSUITEBLADE)
                            {
                                // Windows Server 2003 Web Edition
                                return " Web Edition";
                            }
                            else
                            {
                                // Windows Server 2003 Standard Edition
                                return " Standard Edition";
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the service pack information of the operating system running on this computer.
        /// </summary>
        /// <returns>A string containing the the operating system service pack information.</returns>
        public static string GetOSServicePack()
        {
            OSVERSIONINFOEX versionInfo = new OSVERSIONINFOEX();

            versionInfo.VersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

            if (!GetVersionEx(ref versionInfo))
            {
                return string.Empty;
            }
            else
            {
                return " " + versionInfo.ServicePackVersion;
            }
        }

        /// <summary>
        /// Returns the name of the operating system running on this computer.
        /// </summary>
        /// <returns>A string containing the the operating system name.</returns>
        public static string GetOSName()
        {
            OperatingSystem info = System.Environment.OSVersion;
            string name = "UNKNOWN";

            switch (info.Platform)
            {
                case PlatformID.Win32Windows:
                    
                    switch (info.Version.Minor)
                    {
                        case 0:
                            
                            name = "Windows 95";
                            break;                                

                        case 10:
                            
                            if (info.Version.Revision.ToString() == "2222A")
                            {
                                name = "Windows 98 Second Edition";
                            }
                            else
                            {
                                name = "Windows 98";
                            }

                            break;                            

                        case 90:
                            
                            name = "Windows Me";
                            break;                            
                    }

                    break;                    

                case PlatformID.Win32NT:
                    
                    switch (info.Version.Major)
                    {
                        case 3:
                            
                            name = "Windows NT 3.51";
                            break;                                

                        case 4:
                            
                            name = "Windows NT 4.0";
                            break;                                

                        case 5:
                            
                            if (info.Version.Minor == 0)
                            {
                                name = "Windows 2000";
                            }
                            else if (info.Version.Minor == 1)
                            {
                                name = "Windows XP";
                            }
                            else if (info.Version.Minor == 2)
                            {
                                name = "Windows Server 2003";
                            }

                            break;                       

                        case 6:
                            
                            name = "Windows Vista";

                            break;                                
                    }

                    break;                    
            }

            return name;
        }

        /// <summary>
        /// Determines if the system is running Windows XP
        /// or above
        /// </summary>
        /// <returns>True if system is running XP or above, False otherwise</returns>
        public static bool IsXPOrAbove()
        {
            bool ret = false;
            if (System.Environment.OSVersion.Version.Major > 5)
            {
                ret = true;
            }
            else if ((System.Environment.OSVersion.Version.Major == 5) &&
                (System.Environment.OSVersion.Version.Minor >= 1))
            {
                ret = true;
            }

            return ret;
        }

        #endregion

        #region Private Methods

        [DllImport("kernel32.dll")]
        private static extern bool GetVersionEx(ref OSVERSIONINFOEX versionInfo);

        #endregion

        #region Private Structures

        /// <summary>
        /// Contains OS version info.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            /// <summary>
            /// Version info size.
            /// </summary>
            public int VersionInfoSize;

            /// <summary>
            /// Major version.
            /// </summary>
            public int MajorVersion;

            /// <summary>
            /// Minor version.
            /// </summary>
            public int MinorVersion;

            /// <summary>
            /// Build number.
            /// </summary>
            public int BuildNumber;

            /// <summary>
            /// Platform ID.
            /// </summary>
            public int PlatformID;

            /// <summary>
            /// Service pack version.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string ServicePackVersion;

            /// <summary>
            /// Service pack major.
            /// </summary>
            public short ServicePackMajor;

            /// <summary>
            /// Service pack minor.
            /// </summary>
            public short ServicePackMinor;

            /// <summary>
            /// Suite mask.
            /// </summary>
            public short SuiteMask;

            /// <summary>
            /// Product type.
            /// </summary>
            public byte ProductType;

            /// <summary>
            /// Reserved flag.
            /// </summary>
            public byte Reserved;
        }

        #endregion
    }
}
