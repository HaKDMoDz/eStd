﻿namespace System.Serial
{
    using System;
    using System.Management;
    using System.Numerics;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;

    #region "CONFIGURATION"

    public abstract class BaseConfiguration
    {
        //Put all functions/variables that should be shared with
        //all other classes that inherit this class.
        //
        //note, this class cannot be used as a normal class that
        //you define because it is MustInherit.
        
        #region Fields

        protected internal string _key = "";

        #endregion

        #region Public Properties
        
        /// <summary>
        ///     The key will be stored here
        /// </summary>
        public virtual string Key
        {
            //will be changed in both generating and validating classe.
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
            }
        }
        
        /// <summary>
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual int MachineCode
        {
            get
            {
                return getMachineCode();
            }
        }

        #endregion
        
        // <summary>
        // Read the serial number from the hard disk that keep the bootable partition (boot disk)
        // </summary>
        // <returns>
        // If succedes, returns the string rappresenting the Serial Number.
        // String.Empty if it fails.
        // </returns>

        #region Methods
        
        [SecuritySafeCritical]
        private static string getHddSerialNumber()
        {
            // --- Win32 Disk 
            var searcher = new ManagementObjectSearcher(
                "\\root\\cimv2",
                "select * from Win32_DiskPartition WHERE BootPartition=True");
            
            uint diskIndex = 999;
            foreach (ManagementObject partition in searcher.Get())
            {
                diskIndex = Convert.ToUInt32(partition.GetPropertyValue("Index"));
                break; // TODO: might not be correct. Was : Exit For
            }
            
            // I haven't found the bootable partition. Fail.
            if (diskIndex == 999)
            {
                return string.Empty;
            }
            
            // --- Win32 Disk Drive
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive where Index = " + diskIndex);
            
            string deviceName = "";
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                deviceName = wmi_HD.GetPropertyValue("Name").ToString();
                break; // TODO: might not be correct. Was : Exit For
            }
            
            // I haven't found the disk drive. Fail
            if (string.IsNullOrEmpty(deviceName.Trim()))
            {
                return string.Empty;
            }
            
            // -- Some problems in query parsing with backslash. Using like operator
            if (deviceName.StartsWith("\\\\.\\"))
            {
                deviceName = deviceName.Replace("\\\\.\\", "%");
            }
            
            // --- Physical Media
            searcher =
                new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia WHERE Tag like '" + deviceName + "'");
            string serial = string.Empty;
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                serial = wmi_HD.GetPropertyValue("SerialNumber").ToString();
                break; // TODO: might not be correct. Was : Exit For
            }
        
            return serial;
        }
        
        [SecuritySafeCritical]
        private static int getMachineCode()
        {
            //      * Copyright (C) 2012 Artem Los, All rights reserved.
            //      * 
            //      * This code will generate a 5 digits long key, finger print, of the system
            //      * where this method is being executed. However, that might be changed in the
            //      * hash function "GetStableHash", by changing the amount of zeroes in
            //      * MUST_BE_LESS_OR_EQUAL_TO to the one you want to have. Ex 1000 will return 
            //      * 3 digits long hash.
            //      * 
            //      * Please note, that you might also adjust the order of these, but remember to
            //      * keep them there because as it is stated at 
            //      * (http://www.codeproject.com/Articles/17973/How-To-Get-Hardware-Information-CPU-ID-MainBoard-I)
            //      * the processorID might be the same at some machines, which will generate same
            //      * hashes for several machines.
            //      * 
            //      * The function will probably be implemented into SKGL Project at http://skgl.codeplex.com/
            //      * and Software Protector at http://softwareprotector.codeplex.com/, so I 
            //      * release this code under the same terms and conditions as stated here:
            //      * http://skgl.codeplex.com/license
            //      * 
            //      * Any questions, please contact me at
            //      *  * artem@artemlos.net
            //      
            var m = new methods();
            
            var searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            string collectedInfo = "";
            // here we will put the informa
            foreach (ManagementObject share in searcher.Get())
            {
                // first of all, the processorid
                collectedInfo += share.GetPropertyValue("ProcessorId");
            }
            
            searcher.Query = new ObjectQuery("select * from Win32_BIOS");
            foreach (ManagementObject share in searcher.Get())
            {
                //then, the serial number of BIOS
                collectedInfo += share.GetPropertyValue("SerialNumber");
            }
            
            searcher.Query = new ObjectQuery("select * from Win32_BaseBoard");
            foreach (ManagementObject share in searcher.Get())
            {
                //finally, the serial number of motherboard
                collectedInfo += share.GetPropertyValue("SerialNumber");
            }
            
            // patch luca bernardini
            if (string.IsNullOrEmpty(collectedInfo) | collectedInfo == "00" | collectedInfo.Length <= 3)
            {
                collectedInfo += getHddSerialNumber();
            }
        
            return m.getEightByteHash(collectedInfo, 100000);
        }

        #endregion
    }
        
    public class SerialKeyConfiguration : BaseConfiguration
    {
        #region Fields
            
        private bool[] _Features = new bool[8]
        {
            false, false, false, false, false, false, false, false
            //the default value of the Fetures array.
        };

        private bool _addSplitChar = true;

        #endregion
        
        #region Public Properties
            
        public virtual bool[] Features
        {
            //will be changed in validating class.
            get
            {
                return this._Features;
            }
            set
            {
                this._Features = value;
            }
        }
            
        public bool addSplitChar
        {
            get
            {
                return this._addSplitChar;
            }
            set
            {
                this._addSplitChar = value;
            }
        }

        #endregion
    }
    
    #endregion
        
    #region "ENCRYPTION"
        
    public class Generate : BaseConfiguration
    {
        //this class have to be inherited because of the key which is shared with both encryption/decryption classes.
        
        #region Fields
        
        private readonly methods m = new methods();
        
        private readonly Random r = new Random();
        
        private readonly SerialKeyConfiguration skc = new SerialKeyConfiguration();
        
        private string _secretPhase;
            
        #endregion

        #region Constructors and Destructors
        
        public Generate()
        {
            // No overloads works with Sub New
        }

        public Generate(SerialKeyConfiguration _serialKeyConfiguration)
        {
            this.skc = _serialKeyConfiguration;
        }
        
        #endregion
        
        #region Public Properties
            
        /// <summary>
        ///     If the key is to be encrypted, enter a password here.
        /// </summary>
        public string secretPhase
        {
            get
            {
                return this._secretPhase;
            }
            set
            {
                if (value != this._secretPhase)
                {
                    this._secretPhase = this.m.twentyfiveByteHash(value);
                }
            }
        }
        
        #endregion
        
        #region Public Methods and Operators
            
        /// <summary>
        ///     This function will generate a key.
        /// </summary>
        /// <param name="timeLeft">For instance, 30 days.</param>
        public string doKey(int timeLeft)
        {
            return this.doKey(timeLeft, DateTime.Today); // removed extra argument false
        }
        
        /// <summary>
        /// </summary>
        /// <param name="timeLeft">For instance, 30 days</param>
        /// <param name="useMachineCode">
        ///     Lock a serial key to a specific machine, given its "machine code". Should be 5 digits
        ///     long.
        /// </param>
        /// <returns></returns>
        /// <remarks></remarks>
        public object doKey(int timeLeft, int useMachineCode)
        {
            return this.doKey(timeLeft, DateTime.Today, useMachineCode);
        }
        
        /// <summary>
        ///     This function will generate a key. You may also change the creation date.
        /// </summary>
        /// <param name="timeLeft">For instance, 30 days.</param>
        /// <param name="creationDate">Change the creation date of a key.</param>
        /// <param name="useMachineCode">
        ///     Lock a serial key to a specific machine, given its "machine code". Should be 5 digits
        ///     long.
        /// </param>
        public string doKey(int timeLeft, DateTime creationDate, int useMachineCode = 0)
        {
            if (timeLeft > 999)
            {
                //Checking if the timeleft is NOT larger than 999. It cannot be larger to match the key-length 20.
                throw new ArgumentException("The timeLeft is larger than 999. It can only consist of three digits.");
            }
                
            if (!string.IsNullOrEmpty(this.secretPhase) | this.secretPhase != null)
            {
                //if some kind of value is assigned to the variable "secretPhase", the code will execute it FIRST.
                //the secretPhase shall only consist of digits!
                var reg = new Regex("^\\d$");
                //cheking the string
                if (reg.IsMatch(this.secretPhase))
                {
                    //throwing new exception if the string contains non-numrical letters.
                    throw new ArgumentException("The secretPhase consist of non-numerical letters.");
                }
            }
                    
            //if no exception is thown, do following
            string _stageThree = null;
            if (useMachineCode > 0 & useMachineCode <= 99999)
            {
                _stageThree = this.m._encrypt(
                    timeLeft,
                    this.skc.Features,
                    this.secretPhase,
                    useMachineCode,
                    creationDate);
                // stage one
            }
            else
            {
                _stageThree = this.m._encrypt(
                    timeLeft,
                    this.skc.Features,
                    this.secretPhase,
                    this.r.Next(0, 99999),
                    creationDate);
                // stage one
            }
                           
            //if it is the same value as default, we do not need to mix chars. This step saves generation time.
            
            if (this.skc.addSplitChar)
            {
                // by default, a split character will be addedr
                this.Key = _stageThree.Substring(0, 5) + "-" + _stageThree.Substring(5, 5) + "-" +
                           _stageThree.Substring(10, 5) + "-" + _stageThree.Substring(15, 5);
            }
            else
            {
                this.Key = _stageThree;
            }
    
            //we also include the key in the Key variable to make it possible for user to get his key without generating a new one.
            return this.Key;
        }
    
        #endregion
    }
        
    #endregion

    #region "DECRYPTION"

    public class Validate : BaseConfiguration
    {
        //this class have to be inherited becuase of the key which is shared with both encryption/decryption classes.

        #region Fields

        private readonly methods _a = new methods();

        private string _res = "";
        
        private string _secretPhase = "";
        
        private SerialKeyConfiguration skc = new SerialKeyConfiguration();
        
        #endregion
            
        #region Constructors and Destructors

        public Validate()
        {
            // No overloads works with Sub New
        }
        
        public Validate(SerialKeyConfiguration _serialKeyConfiguration)
        {
            this.skc = _serialKeyConfiguration;
        }
            
        #endregion
                
        #region Public Properties
        
        /// <summary>
        ///     Returns the creation date of the key.
        /// </summary>
        public DateTime CreationDate
        {
            get
            {
                return this._CreationDay();
            }
        }
        
        /// <summary>
        ///     Returns the amount of days the key will be valid.
        /// </summary>
        public int DaysLeft
        {
            get
            {
                return this._DaysLeft();
            }
        }
        
        /// <summary>
        ///     Returns the date when the key is to be expired.
        /// </summary>
        public DateTime ExpireDate
        {
            get
            {
                return this._ExpireDate();
            }
        }
                
        /// <summary>
        ///     Returns all 8 features in a boolean array
        /// </summary>
        public bool[] Features
        {
            //we already have defined Features in the BaseConfiguration class. 
            //Here we only change it to Read Only.
            get
            {
                return this._Features();
            }
        }
        
        /// <summary>
        ///     If the key has expired - returns true; if the key has not expired - returns false.
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return this._IsExpired();
            }
        }
            
        /// <summary>
        ///     If the current machine's machine code is equal to the one that this key is designed for, return true.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsOnRightMachine
        {
            get
            {
                int decodedMachineCode = Convert.ToInt32(this._res.Substring(23, 5));
        
                return decodedMachineCode == this.MachineCode;
            }
        }
            
        /// <summary>
        ///     Checks whether the key has been modified or not. If the key has been modified - returns false; if the key has not
        ///     been modified - returns true.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this._IsValid();
            }
        }
            
        /// <summary>
        ///     Enter a key here before validating.
        /// </summary>
        public new string Key
        {
            //re-defining the Key
            get
            {
                return this._key;
            }
            set
            {
                this._res = "";
                this._key = value;
            }
        }
        
        /// <summary>
        ///     Returns the actual amount of days that were set when the key was generated.
        /// </summary>
        public int SetTime
        {
            get
            {
                return this._SetTime();
            }
        }
            
        /// <summary>
        ///     If the key has been encrypted, when it was generated, please set the same secretPhase here.
        /// </summary>
        public string secretPhase
        {
            get
            {
                return this._secretPhase;
            }
            set
            {
                if (value != this._secretPhase)
                {
                    this._secretPhase = this._a.twentyfiveByteHash(value);
                }
            }
        }
                
        #endregion
                
        #region Methods
            
        private DateTime _CreationDay()
        {
            this.decodeKeyToString();
            var _date = new DateTime();
            _date = new DateTime(
                Convert.ToInt32(this._res.Substring(9, 4)),
                Convert.ToInt32(this._res.Substring(13, 2)),
                Convert.ToInt32(this._res.Substring(15, 2)));

            return _date;
        }
            
        private int _DaysLeft()
        {
            this.decodeKeyToString();
            int _setDays = this.SetTime;
            return Convert.ToInt32((this.ExpireDate - DateTime.Today).TotalDays); //or viseversa
        }
        
        private DateTime _ExpireDate()
        {
            this.decodeKeyToString();
            var _date = new DateTime();
            _date = this.CreationDate;
            return _date.AddDays(this.SetTime);
        }
            
        private bool[] _Features()
        {
            this.decodeKeyToString();
            return this._a.intToBoolean(Convert.ToInt32(this._res.Substring(20, 3)));
        }
        
        private bool _IsExpired()
        {
            if (this.DaysLeft > 0)
            {
                return false;
            }
            return true;
        }
                        
        private bool _IsValid()
        {
            //Dim _a As New methods ' is only here to provide the geteighthashcode method
            try
            {
                if (this.Key.Contains("-"))
                {
                    if (this.Key.Length != 23)
                    {
                        return false;
                    }
                }
                else
                {
                    if (this.Key.Length != 20)
                    {
                        return false;
                    }
                }
                this.decodeKeyToString();
                    
                string _decodedHash = this._res.Substring(0, 9);
                string _calculatedHash = this._a.getEightByteHash(this._res.Substring(9, 19)).ToString().Substring(0, 9);
                // changed Math.Abs(_res.Substring(0, 17).GetHashCode).ToString.Substring(0, 8)
            
                //When the hashcode is calculated, it cannot be taken for sure, 
                //that the same hash value will be generated.
                //learn more about this issue: http://msdn.microsoft.com/en-us/library/system.object.gethashcode.aspx
                if (_decodedHash == _calculatedHash)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                //if something goes wrong, for example, when decrypting, 
                //this function will return false, so that user knows that it is unvalid.
                //if the key is valid, there won't be any errors.
                return false;
            }
        }
            
        private int _SetTime()
        {
            this.decodeKeyToString();
            return Convert.ToInt32(this._res.Substring(17, 3));
        }
                
        private void decodeKeyToString()
        {
            // checking if the key already have been decoded.
            if (string.IsNullOrEmpty(this._res) | this._res == null)
            {
                string _stageOne = "";

                this.Key = this.Key.Replace("-", "");
                
                //if the admBlock has been changed, the getMixChars will be executed.
                    
                _stageOne = this.Key;
                    
                _stageOne = this.Key;
                    
                // _stageTwo = _a._decode(_stageOne)
                        
                if (!string.IsNullOrEmpty(this.secretPhase) | this.secretPhase != null)
                {
                    //if no value "secretPhase" given, the code will directly decrypt without using somekind of encryption
                    //if some kind of value is assigned to the variable "secretPhase", the code will execute it FIRST.
                    //the secretPhase shall only consist of digits!
                    var reg = new Regex("^\\d$");
                    //cheking the string
                    if (reg.IsMatch(this.secretPhase))
                    {
                        //throwing new exception if the string contains non-numrical letters.
                        throw new ArgumentException("The secretPhase consist of non-numerical letters.");
                    }
                }
                this._res = this._a._decrypt(_stageOne, this.secretPhase);
            }
        }
        
        #endregion
    }
            
    #endregion
            
    #region "T H E  C O R E  O F  S K G L"
                
    internal class methods : SerialKeyConfiguration
    {
        //The construction of the key
            
        #region Methods
        
        protected internal string Return_Lenght(string Number, int Lenght)
        {
            // This function create 3 lenght char ex: 39 to 039
            if ((Number.Length != Lenght))
            {
                while (!(Number.Length == Lenght))
                {
                    Number = "0" + Number;
                }
            }
            return Number;
            //Return Number
        }
            
        protected internal string _decText(string _encryptedPhase, string _secretPhase)
        {
            //in this class we are decrypting the text encrypted with the function above.
            string _res = "";
        
            for (int i = 0; i <= _encryptedPhase.Length - 1; i++)
            {
                _res +=
                    this.modulo(
                                Convert.ToInt32(_encryptedPhase.Substring(i, 1)) -
                                Convert.ToInt32(_secretPhase.Substring(this.modulo(i, _secretPhase.Length), 1)),
                        10);
            }
            
            return _res;
        }
        
        protected internal string _decrypt(string _key, string _secretPhase)
        {
            if (string.IsNullOrEmpty(_secretPhase) | _secretPhase == null)
            {
                // if not password is set, return an unencrypted key
                return this.base26ToBase10(_key);
            }
            // if password is set, return an encrypted 
            string usefulInformation = this.base26ToBase10(_key);
            return usefulInformation.Substring(0, 9) + this._decText(usefulInformation.Substring(9), _secretPhase);
        }
                    
        //Deeper - encoding, decoding, et cetera.
                                
        //Convertions, et cetera.----------------
            
        protected internal string _encText(string _inputPhase, string _secretPhase)
        {
            //in this class we are encrypting the integer array.
            string _res = "";
        
            for (int i = 0; i <= _inputPhase.Length - 1; i++)
            {
                _res +=
                    this.modulo(
                                Convert.ToInt32(_inputPhase.Substring(i, 1)) +
                                Convert.ToInt32(_secretPhase.Substring(this.modulo(i, _secretPhase.Length), 1)),
                        10);
            }
            
            return _res;
        }
            
        protected internal string _encrypt(int _days, bool[] _tfg, string _secretPhase, int ID, DateTime _creationDate)
        {
            // This function will store information in Artem's ISF-2
            //Random variable was moved because of the same key generation at the same time.
            int _retInt = Convert.ToInt32(_creationDate.ToString("yyyyMMdd"));
            // today
            
            decimal result = 0;
            
            result += _retInt;
            // adding the current date; the generation date; today.
            result *= 1000;
            // shifting three times at left

            result += _days;
            // adding time left
            result *= 1000;
            // shifting three times at left
                
            result += this.booleanToInt(_tfg);
            // adding features
            result *= 100000;
            //shifting three times at left
            
            result += ID;
            // adding random ID

            // This part of the function uses Artem's SKA-2
        
            if (string.IsNullOrEmpty(_secretPhase) | _secretPhase == null)
            {
                // if not password is set, return an unencrypted key
                return this.base10ToBase26((this.getEightByteHash(result.ToString()) + result.ToString()));
            }
            // if password is set, return an encrypted 
            return
            this.base10ToBase26(
                (this.getEightByteHash(result.ToString()) + this._encText(result.ToString(), _secretPhase)));
        }

        protected internal string base10ToBase26(string s)
        {
            // This method is converting a base 10 number to base 26 number.
            // Remember that s is a decimal, and the size is limited. 
            // In order to get size, type Decimal.MaxValue.
            //
            // Note that this method will still work, even though you only 
            // can add, subtract numbers in range of 15 digits.
            char[] allowedLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            
            decimal num = Convert.ToDecimal(s);
            int reminder = 0;
            
            var result = new char[s.Length + 1];
            int j = 0;

            while ((num >= 26))
            {
                reminder = Convert.ToInt32(num % 26);
                result[j] = allowedLetters[reminder];
                num = (num - reminder) / 26;
                j += 1;
            }
        
            result[j] = allowedLetters[Convert.ToInt32(num)];
            // final calculation
            
            string returnNum = "";
            
            for (int k = j; k >= 0; k -= 1) // not sure
            {
                returnNum += result[k];
            }
            return returnNum;
        }

        protected internal string base26ToBase10(string s)
        {
            // This function will convert a number that has been generated
            // with functin above, and get the actual number in decimal
            //
            // This function requieres Mega Math to work correctly.
            string allowedLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = new BigInteger();
            
            for (int i = 0; i <= s.Length - 1; i += 1)
            {
                BigInteger pow = this.powof(26, (s.Length - i - 1));
            
                result = result + allowedLetters.IndexOf(s.Substring(i, 1)) * pow;
            }
            
            return result.ToString(); //not sure
        }
                    
        protected internal int booleanToInt(bool[] _booleanArray)
        {
            int _aVector = 0;
            //
            //In this function we are converting a binary value array to a int
            //A binary array can max contain 4 values.
            //Ex: new boolean(){1,1,1,1}

            for (int _i = 0; _i < _booleanArray.Length; _i++)
            {
                switch (_booleanArray[_i])
                {
                    case true:
                        _aVector += Convert.ToInt32((Math.Pow(2, (_booleanArray.Length - _i - 1))));
                        // times 1 has been removed
                        break;
                }
            }
            return _aVector;
        }
                
        protected internal int getEightByteHash(string s, int MUST_BE_LESS_THAN = 1000000000)
        {
            //This function generates a eight byte hash
            //The length of the result might be changed to any length
            //just set the amount of zeroes in MUST_BE_LESS_THAN
            //to any length you want
            uint hash = 0;
            
            foreach (byte b in Encoding.Unicode.GetBytes(s))
            {
                hash += b;
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
        
            var result = (int)(hash % MUST_BE_LESS_THAN);
            int check = MUST_BE_LESS_THAN / result;
            
            if (check > 1)
            {
                result *= check;
            }
                
            return result;
        }
        
        protected internal bool[] intToBoolean(int _num)
        {
            //In this function we are converting an integer (created with privious function) to a binary array
            int _bReturn = Convert.ToInt32(Convert.ToString(_num, 2));
            string _aReturn = this.Return_Lenght(_bReturn.ToString(), 8);
            var _cReturn = new bool[8];
            
            for (int i = 0; i <= 7; i++)
            {
                _cReturn[i] = _aReturn.Substring(i, 1) == "1" ? true : false;
            }
            return _cReturn;
        }
            
        protected internal int modulo(int _num, int _base)
        {
            // canged return type to integer.
            //this function simply calculates the "right modulo".
            //by using this function, there won't, hopefully be a negative
            //number in the result!
            return _num - _base * Convert.ToInt32(Math.Floor(_num / (decimal)_base));
        }
            
        protected internal BigInteger powof(int x, int y)
        {
            // Because of the uncertain answer using Math.Pow and ^, 
            // this function is here to solve that issue.
            // It is currently using the MegaMath library to calculate.
            BigInteger newNum = 1;
                
            if (y == 0)
            {
                return 1;
                // if 0, return 1, e.g. x^0 = 1 (mathematicaly proven!) 
            }
            if (y == 1)
            {
                return x;
                // if 1, return x, which is the base, e.g. x^1 = x
            }
            for (int i = 0; i <= y - 1; i++)
            {
                newNum = newNum * x;
            }
            return newNum;
            // if both conditions are not satisfied, this loop
            // will continue to y, which is the exponent.
        }
                
        protected internal string twentyfiveByteHash(string s)
        {
            int amountOfBlocks = s.Length / 5;
            var preHash = new string[amountOfBlocks + 1];

            if (s.Length <= 5)
            {
                //if the input string is shorter than 5, no need of blocks! 
                preHash[0] = this.getEightByteHash(s).ToString();
            }
            else if (s.Length > 5)
            {
                //if the input is more than 5, there is a need of dividing it into blocks.
                for (int i = 0; i <= amountOfBlocks - 2; i++)
                {
                    preHash[i] = this.getEightByteHash(s.Substring(i * 5, 5)).ToString();
                }

                preHash[preHash.Length - 2] =
                    this.getEightByteHash(s.Substring((preHash.Length - 2) * 5, s.Length - (preHash.Length - 2) * 5))
                        .ToString();
            }
            return string.Join("", preHash);
        }

        #endregion
    }

    #endregion
}