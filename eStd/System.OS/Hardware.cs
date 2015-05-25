using System.Management;

namespace Creek.Info
{
    public class Hardware
    {
        public class Keyboard
        {
            public string KeyboardName
            {
                get
                {
                    ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Keyboard");
                    ManagementObjectCollection.ManagementObjectEnumerator enumerator;
                    
                    enumerator = managementObjectSearcher.Get().GetEnumerator();
                    enumerator.MoveNext();
                            
                    return enumerator.Current["Name"].ToString();
                }
            }
        }

        public class Monitor
        {
            public string MonitorName
            {
                get
                {
                    ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2",
                                                                                                     "SELECT * FROM Win32_DesktopMonitor");
                    ManagementObjectCollection.ManagementObjectEnumerator enumerator;
                    enumerator = managementObjectSearcher.Get().GetEnumerator();
                    enumerator.MoveNext();

                    return enumerator.Current["Name"].ToString();
                }
            }
        }

    }
}
