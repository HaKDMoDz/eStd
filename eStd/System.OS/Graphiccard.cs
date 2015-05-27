using System.Management;

namespace System.Computer
{
    public class GraphicCard
    {
        public string Name
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;
                
                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();
                
                return enumerator.Current["Name"].ToString();
            }
        }
        public string DeviceID
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;
                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();

                return enumerator.Current["DeviceID"].ToString();
            }
        }
        public string DriverVersion
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;
                
                enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();

                return enumerator.Current["DriverVersion"].ToString();
            }
        }
    }
}
