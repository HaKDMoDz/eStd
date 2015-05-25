using System.Management;

namespace Creek.Info
{
    public class Bus
    {
        public string BusNum
        {
            get
            {
                var managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Bus");
                var enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();
                
                return enumerator.Current["BusNum"].ToString();    
            }
        }
        public string BusType
        {
            get
            {
                var managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Bus");
                var enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();

                return enumerator.Current["BusType"].ToString();
            }
        }
        public string Description
        {
            get
            {
                var managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Bus");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator();

                enumerator.MoveNext();
                return enumerator.Current["Description"].ToString();

                }
            }
        public string DeviceID
        {
            get
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Bus");
                ManagementObjectCollection.ManagementObjectEnumerator enumerator;
              enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();
                        return enumerator.Current["DeviceID"].ToString();
                }
            }
        
    }
}
