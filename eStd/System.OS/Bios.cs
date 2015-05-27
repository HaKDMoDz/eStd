using System.Management;
using Microsoft.Win32;

namespace System.Computer
{
    public class Bios
    {
        public string Vendor
        { 
            get
            {
                return
                    Registry.GetValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\SYSTEM\\BIOS", "BIOSVendor", (object) null).
                        ToString();
            }
        }
        public string Version
        {
            get
            {
                var managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

                ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();
                
                return enumerator.Current["Version"].ToString();   
            }
        }
        public string SystemFamily
        {
            get
            {
                return
                    Registry.GetValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\SYSTEM\\BIOS", "SystemFamily",
                                      null).ToString();
            }
        }
        public string SystemManufacturer
        {
            get
            {
                return
                    Registry.GetValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\SYSTEM\\BIOS", "SystemManufacturer",
                                      null).ToString();
            }
        }
        public string Name
        {
            get
            {
                var managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

                var enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();
                
                return enumerator.Current["Name"].ToString();
            }
        }
        public string Description
        {
            get
            {
                var managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;
                
                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();

                return enumerator.Current["Description"].ToString();
            }
        }
        public string SerialNumber
        {
            get
            {
                var managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");
                var enumerator = managementObjectSearcher.Get().GetEnumerator();
               
                enumerator.MoveNext();
                
                return enumerator.Current["SerialNumber"].ToString();
            }
        }
    }
}