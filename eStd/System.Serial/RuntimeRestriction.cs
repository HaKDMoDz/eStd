namespace System.Serial
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using Microsoft.VisualBasic;
    using Microsoft.Win32;
    using System.Windows.Forms;

    public class RuntimeRestriction
    {
        #region Fields
        
        private readonly int _days;
        
        #endregion
        
        #region Constructors and Destructors
        
        public RuntimeRestriction(int days)
        {
            this._days = days;
        }
        
        #endregion
        
        #region Delegates
        
        public delegate void RuntimeCheckedEventHandler();
        
        public delegate void RuntimeEndEventHandler();
        
        #endregion
        
        #region Public Events
        
        public event RuntimeCheckedEventHandler RuntimeChecked;
        
        public event RuntimeEndEventHandler RuntimeEnd;
        
        #endregion
        
        #region Public Methods and Operators
        
        public void CheckRuntime()
        {
            string val =
                Registry.GetValue(
                    "HKEY_CURRENT_USER\\" + Assembly.GetCallingAssembly().GetName(),
                    "RuntimeRestriction",
                    "0").ToString();
            if (val == "0")
            {
                MessageBox.Show("Error!");
                return;
            }
            if (this.RuntimeChecked != null)
            {
                this.RuntimeChecked();
            }
            if (DateAndTime.DateDiff(DateInterval.Day, Convert.ToDateTime(this.DCrypt(val)), DateTime.Now) >= this._days)
            {
                if (this.RuntimeEnd != null)
                {
                    this.RuntimeEnd();
                }
            }
        }
        
        public void StartRuntimeRestriction(string appName)
        {
            Registry.SetValue(
                "HKEY_CURRENT_USER\\" + appName,
                "RuntimeRestriction",
                this.Crypt(DateTime.Now.ToString(CultureInfo.InvariantCulture)));
        }
        
        #endregion
        
        #region Methods
        
        private string Crypt(string txt)
        {
            string @out = Convert.ToBase64String(Encoding.Unicode.GetBytes(txt));
            @out = Encoding.Unicode.GetString(Encoding.ASCII.GetBytes(@out));
            return @out;
        }
        
        private string DCrypt(string txt)
        {
            string @out = Encoding.ASCII.GetString(Encoding.Unicode.GetBytes(txt));
            @out = Encoding.Unicode.GetString(Convert.FromBase64String(@out));
            return @out;
        }
    
        #endregion
    }
}