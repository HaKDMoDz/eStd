using System.Management;

namespace System.Computer
{
    public class DiskDrive
    {
        public string Size
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2",
                                                                                                 "SELECT * FROM Win32_DiskDrive");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;
                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();

                return enumerator.Current["Size"].ToString();
                
            }
        }
        public string Name
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2",
                                                                                                 "SELECT * FROM Win32_DiskDrive");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;

                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();

                return enumerator.Current["Name"].ToString();

            }
        }
        public string Partitions
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2",
                                                                                                 "SELECT * FROM Win32_DiskDrive");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;
                
                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();
                
                return enumerator.Current["Partitions"].ToString();
            }
        }
        public string Description
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;
                
                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();
                 
                return enumerator.Current["Description"].ToString();
            }
        }
        public string Manufacturer
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2",
                                                                                                 "SELECT * FROM Win32_DiskDrive");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;

                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();

                return enumerator.Current["Manufacturer"].ToString();
            }
        }
        public string SerialNumber
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;
               
                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();
                
                return enumerator.Current["SerialNumber"].ToString();
            }
        }

        public string Signature
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2",
                                                                                                 "SELECT * FROM Win32_DiskDrive");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;

                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();

                return enumerator.Current["Signature"].ToString();
            }
        }
    }
}
