using System.Management;
using System.Windows.Forms;

namespace System.Computer
{
    public class Battery
    {
        
        public string LineStatus
        {
            get
            {
                return SystemInformation.PowerStatus.PowerLineStatus.ToString();
            }
        }
        public string BatteryLifePercent
        {
            get { return SystemInformation.PowerStatus.BatteryLifePercent * 100f + "%"; }
        }
        public string BatteryChargeStatus
        {
            get
            {
                return SystemInformation.PowerStatus.BatteryChargeStatus.ToString();
            }
        }
        public string BatteryLifeRemaining
        {
            get
            {
                return SystemInformation.PowerStatus.BatteryLifeRemaining.ToString();
            }
        }
        public string BatteryFullLifetime
        {
            get
            {
                return SystemInformation.PowerStatus.BatteryFullLifetime.ToString();
            }
        }
        public string SmartBatteryVersion
        {
            get
            {
                var managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Battery");

                var enumerator = managementObjectSearcher.Get().GetEnumerator();
                enumerator.MoveNext();
                return enumerator.Current["SmartBatteryVersion"].ToString();
            }
        }
    }

}
