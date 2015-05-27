// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED!
// YOU MAY USE THIS CODE: HOWEVER THIS GRANTS NO FUTURE RIGHTS.
// see http://telnetcsharp.codeplex.com/ for further details and license information
namespace System.Net.Telnet
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// Supports telnet connectivity.
    /// <list type="bullet">
    /// <item>Version 0.70: 1st running version</item>
    /// <item>Version 0.71: Telnet class renamed to Terminal and close method improved</item>
    /// <item>Version 0.72: Added custom exceptions which may be used externally, Feedback of Mark H. considered, Wait() method added and <see cref="WaitForChangedScreen(int)"></see> fixed.</item>
    ///	<item>Version 0.73:	Offset problem in Virtual Screen fixed due to mail of Steve, thanks!</item>
    ///	<item>Version 0.74:	SendResponseFunctionKey(int) and fixed WaitFor[XYZ]-methods to better reflect the timeout. Thanks Judah!</item>
    /// <item>Version 0.80: First version going to CodePlex. Implemented fixes as of mail T.N. 16.2.11.</item>
    /// </list>
    /// <list type="number">
    ///		<listheader>
    ///			<term>Features</term>
    ///			<description>Telnet functionality implemented</description>
    ///		</listheader>
    ///		<item>
    ///			<term>LOGOUT</term>
    ///			<description>Logout functionaliy implemented</description>
    ///		</item>
    ///		<item>
    ///			<term>NAWS</term>
    ///			<description>Sends a window size</description>
    ///		</item>
    ///		<item>
    ///			<term>TERMTYPE</term>
    ///			<description>Sends an "ANSI"-terminal type</description>
    ///		</item>
    ///		<item>
    ///			<term>Other telnet commands</term>
    ///			<description>Will be answered correctly with WILL / WONT</description>
    ///		</item>
    ///		<item>
    ///			<term>ESC-Sequences</term>
    ///			<description>Method dealing with ESC-sequences</description>
    ///		</item>
    ///	</list>
    /// </summary>
    /// <remarks>
    /// The class is NOT thread safe for several connections, so each connection should have its own instance.
    /// </remarks>
    public class Terminal : IDisposable
    {
        #region Fields and properties
        private const byte Cr = 13;
        private const string Endofline = "\r\n"; // CR LF
        private const byte Esc = 27;
        private const String F1 = "\033OP"; // function key
        private const String F10 = "\033[21~";
        private const String F11 = "\033[23~";
        private const String F12 = "\033[24~";
        private const String F2 = "\033OQ";
        private const String F3 = "\033OR";
        private const String F4 = "\033OS";
        private const String F5 = "\033[15~";
        private const String F6 = "\033[17~";
        private const String F7 = "\033[18~";
        private const String F8 = "\033[19~";
        private const String F9 = "\033[20~";
        private const byte Lf = 10;
        private const int ReceiveBufferSize = 10*1024; // read a lot
        private const int ScreenXNullcoordinate = 0;
        private const int ScreenYNullCoordinate = 0;
        private const int SendBufferSize = 25; // only small reponses -> only for DOs, WILLs, not for user's responses
        // private const byte TncAo = 245; // F5 The function AO. Abort output
        // private const byte TncAyt = 246; // F6 Are You There The function AYT. 
        // private const byte TncBrk = 243; // F3 Break NVT character BRK.
        // private const byte TncDatamark = 242; // F2 The data stream portion of a Synch. This should always be accompanied by a TCP Urgent notification. 
        private const byte TncDo = 253; // FD Option code: Indicates the request that the other party perform, or confirmation that you are expecting the other party to perform, the indicated option.
        private const byte TncDont = 254; // FE Option code: Indicates the demand that the other party stop performing, or confirmation that you are no longer expecting the other party to perform, the indicated option.
        // private const byte TncEc = 247; // F7 Erase character. The function EC. 
        // private const byte TncEl = 248; // F8 Erase line. The function EL.
        // private const byte TncGa = 249; // F9 Go ahead The GA signal. 
        private const byte TncIac = 255; // FF Data Byte 255
        // private const byte TncIp = 244; // F4 Interrupt Process The function IP. 
        // private const byte TncNop = 241; // No operation
        private const byte TncSb = 250; // FA Option code: Indicates that what follows is subnegotiation of the indicated option.
        private const byte TncSe = 240; // End of subnegotiation parameters
        private const byte TncWill = 251; // FB Option code: Indicates the desire to begin performing, or confirmation that you are now performing, the indicated option.
        private const byte TncWont = 252; // FC Option code: Indicates the refusal to perform, or continue performing, the indicated option.
        private const byte TnoEcho = 1; // 00 echo
        private const byte TnoLogout = 18; // 12 Logout
        private const byte TnoNaws = 31; // 1F Window size
        private const byte TnoNewenv = 39; // 27 New environment option
        private const byte TnoRemoteflow = 33; // 21 Remote flow control
        private const byte TnoTermspeed = 32; // 20 Terminal speed
        private const byte TnoTermtype = 24; // 18 Terminal size
        // private const byte TnoTransbin = 0; // 00 transmit binary
        private const byte TnoXdisplay = 35; // 23 X-Display location
        private const byte TnxIs = 0; // 00 is, e.g. used with SB terminal type
        // private const byte TnxSend = 1; // 01 send, e.g. used with SB terminal type
        private const int Trails = 25; // trails until timeout in "wait"-methods
        /// <summary>The version</summary>
        public const String Version = "0.74";
        private static readonly Regex RegExpCursorLeft = new Regex("\\[\\d*D", RegexOptions.Compiled);
        private static readonly Regex RegExpCursorPosition = new Regex("\\[\\d*;\\d*[Hf]", RegexOptions.Compiled);
        private static readonly Regex RegExpCursorRight = new Regex("\\[\\d*C", RegexOptions.Compiled);
        private static readonly Regex RegExpCursorXPosition = new Regex(";\\d+[Hf]", RegexOptions.Compiled); // column
        private static readonly Regex RegExpCursorYPosition = new Regex("\\[\\d+;", RegexOptions.Compiled); // line
        private static readonly Regex RegExpIp = new Regex(@"\d?\d?\d\.\d?\d?\d\.\d?\d?\d\.\d?\d?\d", RegexOptions.Compiled);
        private static readonly Regex RegExpNumber = new Regex("\\d+", RegexOptions.Compiled);
        private static readonly Regex RegExpScrollingRegion = new Regex("\\[\\d*;\\d*r", RegexOptions.Compiled);
        private readonly string _hostName;
        private readonly int _port;
        private readonly int _timeoutReceive; // timeout in seconds
        private readonly int _timeoutSend; // timeout in seconds for TCP client
        private readonly int _vsHeight;
        private readonly int _vsWidth;
        private byte[] _buffer;
        private AsyncCallback _callBackReceive; // callback method
        private AsyncCallback _callBackSend; // callback method
        private bool _clientInitNaws;
        private bool _firstResponse = true;
        private bool _forceLogout;
        private bool _nawsNegotiated;
        private bool _serverEcho;
        private TcpClient _tcpClient;
        private VirtualScreen _virtualScreen;
        /// <summary>
        /// Property virtual screen
        /// </summary>
        public VirtualScreen VirtualScreen { get { return this._virtualScreen; } }
        /// <summary>
        /// Server echo on?
        /// </summary>
        public bool EchoOn { get { return this._serverEcho; } }
        #endregion Fields / Properties

        #region Constructors and destructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostName">IP address, e.g. 192.168.0.20</param>
        public Terminal(string hostName) : this(hostName, 23, 10, 80, 40)
        {
            // nothing further
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostName">IP address, e.g. 192.168.0.20</param>
        /// <param name="port">Port, usually 23 for telnet</param>
        /// <param name="timeoutSeconds">Timeout for connections [s], both read and write</param>
        /// <param name="virtualScreenWidth">Screen width for the virtual screen</param>
        /// <param name="virtualScreenHeight">Screen height for the virtual screen</param>
        public Terminal(string hostName, int port, int timeoutSeconds, int virtualScreenWidth, int virtualScreenHeight)
        {
            this._hostName = hostName;
            this._port = port;
            this._timeoutReceive = timeoutSeconds;
            this._timeoutSend = timeoutSeconds;
            this._serverEcho = false;
            this._clientInitNaws = false;
            this._firstResponse = true;
            this._nawsNegotiated = false;
            this._forceLogout = false;
            this._vsHeight = virtualScreenHeight;
            this._vsWidth = virtualScreenWidth;
        }

        /// <summary>
        /// Dispose part, calls Close()
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Destructor, calls Close()
        /// </summary>
        ~Terminal()
        {
            this.Close();
        }
        #endregion

        #region Connect / Close
        /// <summary>
        /// Connect to the telnet server
        /// </summary>
        /// <returns>true if connection was successful</returns>
        public bool Connect()
        {
            // check for buffer
            if (this._buffer == null)
                this._buffer = new byte[ReceiveBufferSize];

            // virtual screen
            if (this._virtualScreen == null)
                this._virtualScreen = new VirtualScreen(this._vsWidth, this._vsHeight, 1, 1);

            // set the callbacks
            if (this._callBackReceive == null)
                this._callBackReceive = this.ReadFromStream;
            if (this._callBackSend == null)
                this._callBackSend = this.WriteToStream;

            // flags
            this._serverEcho = false;
            this._clientInitNaws = false;
            this._firstResponse = true;
            this._nawsNegotiated = false;
            this._forceLogout = false;

            // return physical connection
            if (this._tcpClient != null)
                return true; // we still have a connection -> ?? better reconnect ??
            try
            {
                // TODO: Improve performance ...?
                // This is not working:	IPAddress ipAddress = IPAddress.Parse(this.hostName);
                //						IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, this.port);
                // because it addresses local endpoints
                this._tcpClient = new TcpClient(this._hostName, this._port) {ReceiveTimeout = this._timeoutReceive, SendTimeout = this._timeoutSend, NoDelay = true};
                this._tcpClient.GetStream().BeginRead(this._buffer, 0, this._buffer.Length, this._callBackReceive, null);
                return true;
            }
            catch
            {
                this._tcpClient = null;
                return false;
            }
        }

        /// <summary>
        /// Closes external resources.
        /// Safe, can be called multiple times
        /// </summary>
        public void Close()
        {
            // physical connection
            if (this._tcpClient != null)
            {
                try
                {
                    // it is important to close the stream
                    // because somehow tcpClient does not physically breaks down
                    // the connection - on "one connection" telnet server the 
                    // server remains blocked if not doing it!
                    this._tcpClient.GetStream().Close();
                    this._tcpClient.Close();
                    this._tcpClient = null;
                }
                catch
                {
                    this._tcpClient = null;
                }
            }

            // clean up
            // fast, "can be done several" times
            this._virtualScreen = null;
            this._buffer = null;
            this._callBackReceive = null;
            this._callBackSend = null;
            this._forceLogout = false;
        }

        // Close()

        /// <summary>
        /// Is connection still open?
        /// </summary>
        /// <returns>true if connection is open</returns>
        public bool IsOpenConnection
        {
            get
            {
                return (this._tcpClient != null);
            }
        }
        #endregion

        #region Send response to Telnet server
        /// <summary>
        /// Send a response to the server
        /// </summary>
        /// <param name="response">response String</param>
        /// <param name="endLine">terminate with appropriate end-of-line chars</param>
        /// <returns>true if sending was OK</returns>
        public bool SendResponse(string response, bool endLine)
        {
            try
            {
                if (!this.IsOpenConnection || this._tcpClient == null) return false;
                if (string.IsNullOrEmpty(response)) return true; // nothing to do
                byte[] sendBuffer = (endLine) ? Encoding.ASCII.GetBytes(response + Endofline) : Encoding.ASCII.GetBytes(response);
                if (sendBuffer.Length < 1) return false;
                this._tcpClient.GetStream().BeginWrite(sendBuffer, 0, sendBuffer.Length, this._callBackSend, null);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Send function key response to Telnet server
        /// <summary>
        /// Send a Funktion Key response to the server
        /// </summary>
        /// <param name="key">Key number 1-12</param>
        /// <returns>true if sending was OK</returns>
        public bool SendResponseFunctionKey(int key)
        {
            if (key < 1 || key > 12)
                return false;
            switch (key)
            {
                case 1:
                    return this.SendResponse(F1, false);
                case 2:
                    return this.SendResponse(F2, false);
                case 3:
                    return this.SendResponse(F3, false);
                case 4:
                    return this.SendResponse(F4, false);
                case 5:
                    return this.SendResponse(F5, false);
                case 6:
                    return this.SendResponse(F6, false);
                case 7:
                    return this.SendResponse(F7, false);
                case 8:
                    return this.SendResponse(F8, false);
                case 9:
                    return this.SendResponse(F9, false);
                case 10:
                    return this.SendResponse(F10, false);
                case 11:
                    return this.SendResponse(F11, false);
                case 12:
                    return this.SendResponse(F12, false);
                default:
                    // this should never be reached
                    return false;
            }
        }
        #endregion

        #region Send Telnet logout sequence
        /// <summary>
        /// Send a synchronously telnet logout-response
        /// </summary>
        /// <returns></returns>
        public bool SendLogout()
        {
            return this.SendLogout(true);
        }

        /// <summary>
        /// Send a telnet logout-response
        /// </summary>
        /// <param name="synchronous">Send synchronously (true) or asynchronously (false)</param>
        /// <returns></returns>
        public bool SendLogout(bool synchronous)
        {
            byte[] lo = {TncIac, TncDo, TnoLogout};
            try
            {
                if (synchronous)
                {
                    this._tcpClient.GetStream().Write(lo, 0, lo.Length);
                }
                else
                {
                    this._tcpClient.GetStream().BeginWrite(lo, 0, lo.Length, this._callBackSend, null);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // sendLogout
        #endregion

        #region WaitFor-methods
        /// <summary>
        /// Wait for a particular string
        /// </summary>
        /// <param name="searchFor">string to be found</param>
        /// <returns>string found or null if not found</returns>
        public string WaitForString(string searchFor)
        {
            return this.WaitForString(searchFor, false, this._timeoutReceive);
        }

        /// <summary>
        /// Wait for a particular string
        /// </summary>
        /// <param name="searchFor">string to be found</param>
        /// <param name="caseSensitive">case sensitive search</param>
        /// <param name="timeoutSeconds">timeout [s]</param>
        /// <returns>string found or null if not found</returns>
        public string WaitForString(string searchFor, bool caseSensitive, int timeoutSeconds)
        {
            if (this._virtualScreen == null || searchFor == null || searchFor.Length < 1)
                return null;
            // use the appropriate timeout setting, which is the smaller number
            int sleepTimeMs = this.GetWaitSleepTimeMs(timeoutSeconds);
            DateTime endTime = this.TimeoutAbsoluteTime(timeoutSeconds);
            do
            {
                string found;
                lock (this._virtualScreen)
                {
                    found = this._virtualScreen.FindOnScreen(searchFor, caseSensitive);
                }
                if (found != null)
                    return found;
                Thread.Sleep(sleepTimeMs);
            }
            while (DateTime.Now <= endTime);
            return null;
        }

        /// <summary>
        /// Wait for a particular regular expression
        /// </summary>
        /// <param name="regEx">string to be found</param>
        /// <returns>string found or null if not found</returns>
        public string WaitForRegEx(string regEx)
        {
            return this.WaitForRegEx(regEx, this._timeoutReceive);
        }

        /// <summary>
        /// Wait for a particular regular expression
        /// </summary>
        /// <param name="regEx">string to be found</param>
        /// <param name="timeoutSeconds">timeout [s]</param>
        /// <returns>string found or null if not found</returns>
        public string WaitForRegEx(string regEx, int timeoutSeconds)
        {
            if (this._virtualScreen == null || regEx == null || regEx.Length < 1)
                return null;
            int sleepTimeMs = this.GetWaitSleepTimeMs(timeoutSeconds);
            DateTime endTime = this.TimeoutAbsoluteTime(timeoutSeconds);
            do // at least once
            {
                string found;
                lock (this._virtualScreen)
                {
                    found = this._virtualScreen.FindRegExOnScreen(regEx);
                }
                if (found != null)
                    return found;
                Thread.Sleep(sleepTimeMs);
            }
            while (DateTime.Now <= endTime);
            return null;
        }

        /// <summary>
        /// Wait for changed screen. Read further documentation 
        /// on <code>WaitForChangedScreen(int)</code>.
        /// </summary>
        /// <returns>changed screen</returns>
        public bool WaitForChangedScreen()
        {
            return this.WaitForChangedScreen(this._timeoutReceive);
        }

        /// <summary>
        /// Waits for changed screen: This method here resets
        /// the flag of the virtual screen and afterwards waits for
        /// changes.
        /// <p>
        /// This means the method detects changes after the call
        /// of the method, NOT prior.
        /// </p>
        /// <p>
        /// To reset the flag only use <code>WaitForChangedScreen(0)</code>.
        /// </p>
        /// </summary>
        /// <param name="timeoutSeconds">timeout [s]</param>
        /// <remarks>
        /// The property ChangedScreen of the virtual screen is
        /// reset after each call of Hardcopy(). It is also false directly
        /// after the initialization.
        /// </remarks>
        /// <returns>changed screen</returns>
        public bool WaitForChangedScreen(int timeoutSeconds)
        {
            // 1st check
            if (this._virtualScreen == null || timeoutSeconds < 0)
                return false;

            // reset flag: This has been added after the feedback of Mark
            if (this._virtualScreen.ChangedScreen)
                this._virtualScreen.Hardcopy(false);

            // Only reset
            if (timeoutSeconds <= 0)
                return false;

            // wait for changes, the goal is to test at TRAILS times, if not timing out before
            int sleepTimeMs = this.GetWaitSleepTimeMs(timeoutSeconds);
            DateTime endTime = this.TimeoutAbsoluteTime(timeoutSeconds);
            do // run at least once
            {
                lock (this._virtualScreen)
                {
                    if (this._virtualScreen.ChangedScreen)
                        return true;
                }
                Thread.Sleep(sleepTimeMs);
            }
            while (DateTime.Now <= endTime);
            return false;
        }

        // WaitForChangedScreen

        /// <summary>
        /// Wait (=Sleep) for n seconds
        /// </summary>
        /// <param name="seconds">seconds to sleep</param>
        public void Wait(int seconds)
        {
            if (seconds > 0)
                Thread.Sleep(seconds*1000);
        }

        // Wait

        /// <summary>
        /// Helper method: 
        /// Get the appropriate timeout, which is the bigger number of
        /// timeoutSeconds and this.timeoutReceive (TCP client timeout)
        /// </summary>
        /// <param name="timeoutSeconds">timeout in seconds</param>
        private int GetWaitTimeout(int timeoutSeconds)
        {
            if (timeoutSeconds < 0 && this._timeoutReceive < 0) return 0;
            if (timeoutSeconds < 0) return this._timeoutReceive; // no valid timeout, return other one
            return (timeoutSeconds >= this._timeoutReceive) ? timeoutSeconds : this._timeoutReceive;
        }

        /// <summary>
        /// Helper method: 
        /// Get the appropriate sleep time based on timeout and TRIAL
        /// </summary>
        /// <param name="timeoutSeconds">timeout ins seconds</param>
        private int GetWaitSleepTimeMs(int timeoutSeconds)
        {
            return (this.GetWaitTimeout(timeoutSeconds)*1000)/Trails;
        }

        /// <summary>
        /// Helper method: 
        /// Get the end time, which is "NOW" + timeout
        /// </summary>
        /// <param name="timeoutSeconds">timeout int seconds</param>
        private DateTime TimeoutAbsoluteTime(int timeoutSeconds)
        {
            return DateTime.Now.AddSeconds(this.GetWaitTimeout(timeoutSeconds));
        }
        #endregion

        #region Callback function ReadFromStream
        /// <summary>
        /// Callback function to read from the network stream
        /// </summary>
        /// <param name="asyncResult">Callback result</param>
        private void ReadFromStream(IAsyncResult asyncResult)
        {
            if (asyncResult == null || this._tcpClient == null)
            {
                this.Close();
                return;
            }

            // read
            try
            {
                // bytes read
                // NOT needed: this.callBackReceive.EndInvoke(asyncResult); -> exception
                int bytesRead = this._tcpClient.GetStream().EndRead(asyncResult);

                if (bytesRead > 0)
                {
                    // Translate the data and write output to Virtual Screen
                    // DO this thread save to make sure we do not "READ" from screen meanwhile
                    lock (this._virtualScreen)
                    {
                        this.ParseAndRespondServerStream(bytesRead);
                    }

                    // Reinitialize callback
                    this.CleanBuffer(bytesRead);
                    if (this._forceLogout)
                        this.Close();
                    else
                        this._tcpClient.GetStream().BeginRead(this._buffer, 0, this._buffer.Length, this._callBackReceive, null);
                }
                else
                    // the connection was terminated by the server
                    this.Close();
            }
            catch
            {
                // the connection was terminated by the server
                this.Close();
            }
        }
        #endregion

        #region Callback function: Write to stream
        /// <summary>
        /// Callback function to write to the network stream
        /// </summary>
        /// <param name="asyncResult">Callback result</param>
        private void WriteToStream(IAsyncResult asyncResult)
        {
            if (asyncResult == null || this._tcpClient == null)
            {
                this.Close();
                return;
            }

            // write 
            try
            {
                this._tcpClient.GetStream().EndWrite(asyncResult);
            }
            catch
            {
                this.Close(); // the connection was terminated by the server
            }
        }

        // write network stream
        #endregion

        #region ParseAndRespondServerStream
        /// <summary>
        /// Go thru the data received and answer all technical server
        /// requests (TELNET negotiations).
        /// </summary>
        /// <param name="bytesRead">number of bytes read</param>
        /// <remarks>
        /// Thread saftey regarding the virtual screen needs to be considered
        /// </remarks>
        private void ParseAndRespondServerStream(int bytesRead)
        {
            // reponse to server
            var response = new MemoryStream(SendBufferSize); // answer usually will be small: "a few bytes only"

            // cycle thru the buffer
            int bc = 0;
            while (this._buffer != null && bc < bytesRead && bc < this._buffer.Length)
            {
                try
                {
                    switch (this._buffer[bc])
                    {
                            // ESC
                        case Esc:
                            bc = this.ParseEscSequence(bc, response);
                            break;
                        case Cr:
                            this._virtualScreen.WriteByte(Cr);
                            break;
                        case Lf:
                            this._virtualScreen.WriteByte(Lf);
                            break;
                            // DO
                        case TncIac:
                            bc++;
                            switch (this._buffer[bc])
                            {
                                case TncDo:
                                    bc++;
                                    switch (this._buffer[bc])
                                    {
                                            // DO ...
                                        case TnoTermspeed:
                                            TelnetWont(TnoTermspeed, response); // no negotiation about speed
                                            break;
                                        case TnoNaws:
                                            if (!this._clientInitNaws)
                                                TelnetWill(TnoNaws, response); // negotiation about window size
                                            TelnetSubNaws(this._virtualScreen.Width, this._virtualScreen.Height, response);
                                            this._nawsNegotiated = true;
                                            break;
                                        case TnoTermtype:
                                            TelnetWill(TnoTermtype, response); // negotiation about terminal type
                                            break;
                                        case TnoXdisplay:
                                            TelnetWont(TnoXdisplay, response); // no negotiation about X-Display
                                            break;
                                        case TnoNewenv:
                                            TelnetWont(TnoNewenv, response); // no environment
                                            break;
                                        case TnoEcho:
                                            TelnetWont(TnoEcho, response); // no echo from client
                                            break;
                                        case TnoRemoteflow:
                                            TelnetWont(TnoRemoteflow, response); // no echo from client
                                            break;
                                        case TnoLogout:
                                            TelnetWill(TnoLogout, response); // no echo from client
                                            this._forceLogout = true;
                                            break;
                                        default:
                                            // all we do not understand =>
                                            // WONT
                                            TelnetWont(this._buffer[bc], response); // whatever -> WONT
                                            break;
                                    } // SWITCH DO XX
                                    break;
                                    // DONT
                                case TncDont:
                                    bc++; // ignore DONTs
                                    break;
                                    // SUB-NEGOTIATION
                                case TncSb:
                                    bc++;
                                    switch (this._buffer[bc])
                                    {
                                            // SUB ...
                                        case TnoTermtype:
                                            bc++; // the follwing byte is usually a send-request ("SEND"), just read the byte 
                                            TelnetSubIsAnsi(response);
                                            break;
                                        case TnoNaws:
                                            bc++; // the follwing byte is usually a send-request ("SEND"), just read the byte 
                                            TelnetSubNaws(this._virtualScreen.Width, this._virtualScreen.Height, response);
                                            this._nawsNegotiated = true;
                                            break;
                                        default:
                                            // read until the end of the subrequest
                                            while (this._buffer[bc] != TncSe && bc < this._buffer.Length)
                                            {
                                                bc++;
                                            }
                                            break;
                                    } // SUB NEG XX
                                    break;
                                    // WILL AND WONTs FROM SERVER
                                case TncWill:
                                    // Server's WILLs
                                    bc++;
                                    switch (this._buffer[bc])
                                    {
                                        case TnoEcho:
                                            this._serverEcho = true;
                                            TelnetDo(this._buffer[bc], response);
                                            break;
                                        case TnoLogout:
                                            // usually a reponse on my logout
                                            // I do no say anything but force the end
                                            this._forceLogout = true;
                                            break;
                                        default:
                                            // other WILLs OF SERVER -> confirm
                                            TelnetDo(this._buffer[bc], response);
                                            break;
                                    }
                                    break;
                                case TncWont:
                                    bc++;
                                    // Server's WONTs
                                    bc++;
                                    switch (this._buffer[bc])
                                    {
                                        case TnoEcho:
                                            this._serverEcho = false;
                                            break;
                                    }
                                    break;
                            }
                            break;
                        default:
                            // no command, data
                            this._virtualScreen.WriteByte(this._buffer[bc]);
                            break;
                    } // switch
                    bc++;
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            } // while

            //
            // send the response
            //
            if (this._firstResponse)
            {
                // send some own WILLs even if not asked as DOs
                if (!this._nawsNegotiated)
                {
                    TelnetWill(TnoNaws, response);
                    this._clientInitNaws = true;
                }
            } // 1st response

            //
            // respond synchronously !
            // -> we know that we really have send the bytes
            //
            if (response.Position > 0)
            {
                byte[] r = MemoryStreamToByte(response);
                if (r != null && r.Length > 0)
                {
                    this._tcpClient.GetStream().Write(r, 0, r.Length);
                    this._tcpClient.GetStream().Flush();
                    this._firstResponse = false;
                }
            }

            // clean up
            try
            {
                response.Close();
            }
                // ReSharper disable EmptyGeneralCatchClause
            catch
            {
                // ignore
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        // method
        #endregion

        #region ParseEscSequence
        /// <summary>
        /// Deal with ESC Sequences as in VT100, ..
        /// </summary>
        /// <param name="bc">current buffer counter</param>
        /// <param name="response">Stream for the response (back to Telnet server)</param>
        /// <returns>new buffer counter (last byte dealed with)</returns>
        /// <remarks>
        /// Thread saftey regarding the virtual screen needs to be considered
        /// </remarks>
        private int ParseEscSequence(int bc, MemoryStream response)
        {
            // some sequences can only be terminated by the end characters
            // (they contain wildcards) => 
            // a switch / case decision is not feasible
            if (this._buffer == null) return bc;

            // find the byte after ESC
            if (this._buffer[bc] == Esc) bc++;

            // now handle sequences
            int m;

            // Cursor Movement Commands 
            //  Index                           ESC D
            //  Next Line                       ESC E
            //  Reverse index                   ESC M
            //  Save cursor and attributes      ESC 7
            //  Restore cursor and attributes   ESC 8
            if ((m = this.MatchSequence(bc, "D")) > -1) return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "E")) > -1)
            {
                this._virtualScreen.CursorNextLine();
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "M")) > -1) return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "7")) > -1) return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "8")) > -1) return (bc + m - 1);

            // Cursor movements
            //  Cursor forward (right)          ESC [ Pn C, e.g. "[12C"	
            //  Cursor backward (left)          ESC [ Pn D,	e.g. "[33D"
            //	Direct cursor addressing        ESC [ Pl; Pc H  or
            //									ESC [ Pl; Pc f
            // Reg Exp: \[ = [  \d = 0-9  + 1 time or more
            if ((m = this.MatchRegExp(bc, RegExpCursorLeft)) > -1)
            {
                this._virtualScreen.MoveCursor(-1);
                return (bc + m - 1);
            }
            if ((m = this.MatchRegExp(bc, RegExpCursorRight)) > -1)
            {
                this._virtualScreen.MoveCursor(1);
                return (bc + m - 1);
            }
            if ((m = this.MatchRegExp(bc, RegExpCursorPosition)) > -1)
            {
                string sequence = Encoding.ASCII.GetString(this._buffer, bc, m);
                int nx = NewCursorXPosition(sequence);
                int ny = NewCursorYPosition(sequence);
                this._virtualScreen.MoveCursorTo(nx, ny);
                return (bc + m - 1);
            }
            // Scrolling region 
            //  ESC [ Pt ; Pb r
            if ((m = this.MatchRegExp(bc, RegExpScrollingRegion)) > -1)
            {
                return (bc + m - 1);
            }
            // Line Size (Double-Height and Double-Width) Commands
            //  Change this line to double-height top half      ESC # 3
            //  Change this line to double-height bottom half   ESC # 4
            //  Change this line to single-width single-height  ESC # 5
            //  Change this line to double-width single-height  ESC # 6
            if ((m = this.MatchSequence(bc, "#3")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "#4")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "#5")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "#6")) > -1)
                return (bc + m - 1);
            // Erasing
            //  From cursor to end of line              ESC [ K  or ESC [ 0 K
            //  From beginning of line to cursor        ESC [ 1 K
            //  Entire line containing cursor           ESC [ 2 K
            //  From cursor to end of screen            ESC [ J  or ESC [ 0 J
            //  From beginning of screen to cursor      ESC [ 1 J
            //  Entire screen                           ESC [ 2 J
            if ((m = this.MatchSequence(bc, "[K")) > -1)
            {
                this._virtualScreen.CleanLine(this._virtualScreen.CursorX, this._virtualScreen.CursorXRight);
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "[0K")) > -1)
            {
                this._virtualScreen.CleanLine(this._virtualScreen.CursorX, this._virtualScreen.CursorXRight);
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "[1K")) > -1)
            {
                this._virtualScreen.CleanLine(this._virtualScreen.CursorXLeft, this._virtualScreen.CursorX);
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "[2K")) > -1)
            {
                this._virtualScreen.CleanLine(this._virtualScreen.CursorXLeft, this._virtualScreen.CursorXRight);
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "[J")) > -1)
            {
                this._virtualScreen.CleanFromCursor();
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "[0J")) > -1)
            {
                this._virtualScreen.CleanFromCursor();
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "[1J")) > -1)
            {
                this._virtualScreen.CleanToCursor();
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "[2J")) > -1)
            {
                // erase entire screen
                this._virtualScreen.CleanScreen();
                return (bc + m - 1);
            }
            // Hardcopy                ESC # 7
            // Graphic processor ON    ESC 1
            // Graphic processor OFF   ESC 2
            if ((m = this.MatchSequence(bc, "#7")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "1")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "2")) > -1)

                return (bc + m - 1);

            // TAB stops
            //  Set tab at current column               ESC H
            //  Clear tab at curent column              ESC [ g or ESC [ 0 g
            //  Clear all tabs                          ESC [ 3 g
            if ((m = this.MatchSequence(bc, "H")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[g")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[0g")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[3g")) > -1)
                return (bc + m - 1);

            // Modes:
            //  Line feed/new line   New line    ESC [20h   Line feed   ESC [20l
            //  Cursor key mode      Application ESC [?1h   Cursor      ESC [?1l
            //  ANSI/VT52 mode       ANSI        N/A        VT52        ESC [?2l
            //  Column mode          132 Col     ESC [?3h   80 Col      ESC [?3l
            //  Scrolling mode       Smooth      ESC [?4h   Jump        ESC [?4l
            //  Screen mode          Reverse     ESC [?5h   Normal      ESC [?5l
            //  Origin mode          Relative    ESC [?6h   Absolute    ESC [?6l
            //  Wraparound           On          ESC [?7h   Off         ESC [?7l
            //  Auto repeat          On          ESC [?8h   Off         ESC [?8l
            //  Interlace            On          ESC [?9h   Off         ESC [?9l
            //  Graphic proc. option On          ESC 1      Off         ESC 2
            //  Keypad mode          Application ESC =      Numeric     ESC >
            if ((m = this.MatchSequence(bc, "[20h")) > -1)
            {
                this._virtualScreen.CursorNextLine();
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "[20l")) > -1)
            {
                response.WriteByte(10);
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "[?1h")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?1l")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?3h")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?3l")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?4h")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?4l")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?5h")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?5l")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?6h")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?6l")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?7h")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?7l")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?8h")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?8l")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?9h")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "[?9l")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "1")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "2")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "=")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, ">")) > -1)
                return (bc + m - 1);

            // VT 52 compatibility mode
            //  Cursor Up                               ESC A
            //  Cursor Down                             ESC B
            //  Cursor Right                            ESC C
            //  Cursor Left                             ESC D
            //  Select Special Graphics character set   ESC F
            //  Select ASCII character set              ESC G
            //  Cursor to home                          ESC H
            //  Reverse line feed                       ESC I
            //  Erase to end of screen                  ESC J
            //  Erase to end of line                    ESC K
            //  Direct cursor address                   ESC Ylc         (see note 1)
            //  Identify                                ESC Z           (see note 2)
            //  Enter alternate keypad mode             ESC =
            //  Exit alternate keypad mode              ESC >
            //  Enter ANSI mode                         ESC <
            if ((m = this.MatchSequence(bc, "A")) > -1)
            {
                this._virtualScreen.MoveCursorVertical(-1);
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "B")) > -1)
            {
                this._virtualScreen.MoveCursorVertical(1);
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "C")) > -1)
            {
                this._virtualScreen.MoveCursorVertical(1);
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "D")) > -1)
            {
                this._virtualScreen.MoveCursorVertical(-1);
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "F")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "G")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "H")) > -1)
            {
                this._virtualScreen.CursorReset();
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "I")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "J")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "K")) > -1)
            {
                this._virtualScreen.CleanLine(this._virtualScreen.CursorX, this._virtualScreen.CursorXRight);
                return (bc + m - 1);
            }
            if ((m = this.MatchSequence(bc, "Ylc")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "Z")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "=")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, "<")) > -1)
                return (bc + m - 1);
            if ((m = this.MatchSequence(bc, ">")) > -1)
                return (bc + m - 1);
            // nothing found
            return bc;
        }
        #endregion ParseEscSequence

        #region MatchSequence- and MatchRegEx-methods
        /// <summary>
        /// Does the sequence match the buffer starting at 
        /// current index?
        /// </summary>
        /// <param name="bufferCounter">Current buffer counter</param>
        /// <param name="sequence">Bytes need to match</param>
        /// <param name="ignoreIndex">Index of bytes which do not NEED to match, e.g. 2 means the 3rd byte (index 2) does not need to match</param>
        /// <returns>Number of characters matching</returns>
        private int MatchSequence(int bufferCounter, byte[] sequence, int[] ignoreIndex = null)
        {
            if (sequence == null || this._buffer == null)
                return -1;
            if (this._buffer.Length < (bufferCounter + sequence.Length))
                return -1; // overflow
            for (int i = 0; i < sequence.Length; i++)
            {
                if (this._buffer[bufferCounter + i] != sequence[i])
                {
                    // not a match
                    if (ignoreIndex == null || ignoreIndex.Length < 1)
                        return -1; // no wildcards
                    bool wildcard = false;
                    // ReSharper disable LoopCanBeConvertedToQuery
                    foreach (int t in ignoreIndex)
                    {
                        if (t == i)
                        {
                            wildcard = true;
                            break;
                        }
                    }
                    // ReSharper restore LoopCanBeConvertedToQuery

                    if (!wildcard)
                        return -1; // no wildcard found
                } // no match
            }
            return sequence.Length;
        }

        /// <summary>
        /// Does the sequence match the buffer?
        /// </summary>
        /// <param name="bufferCounter">Current buffer counter</param>
        /// <param name="sequence">String needs to match</param>
        /// <returns>Number of characters matching</returns>
        private int MatchSequence(int bufferCounter, string sequence)
        {
            if (sequence == null)
                return -1;
            return this.MatchSequence(bufferCounter, Encoding.ASCII.GetBytes(sequence));
        }

        /// <summary>
        /// Match a regular Expression
        /// </summary>
        /// <param name="bufferCounter">Current buffer counter</param>
        /// <param name="r">Regular expression object</param>
        /// <returns>Number of characters matching</returns>
        private int MatchRegExp(int bufferCounter, Regex r)
        {
            if (r == null || this._buffer == null || this._buffer.Length < bufferCounter)
                return -1;
            // build a dummy string
            // which can be checked
            const int dsl = 10;
            string dummyString = this._buffer.Length >= (bufferCounter + dsl) ? Encoding.ASCII.GetString(this._buffer, bufferCounter, dsl) : Encoding.ASCII.GetString(this._buffer, bufferCounter, this._buffer.Length - bufferCounter);
            if (String.IsNullOrEmpty(dummyString)) return -1;
            Match m = r.Match(dummyString);
            if (m.Success && m.Index == 0) return m.Length;
            return -1;
        }
        #endregion

        #region Cursor movements in virtual screen
        /// <summary>
        /// Find the X position in a VT cursor position sequence.
        /// This only works if the sequence is a valid position sequence!
        /// </summary>
        /// <param name="escSequence">Valid position sequence</param>
        /// <returns>X position (column)</returns>
        private static int NewCursorXPosition(string escSequence)
        {
            const int DEFAULT = ScreenXNullcoordinate;
            if (escSequence == null)
                return -1; // error
            Match m = RegExpCursorXPosition.Match(escSequence);
            if (!m.Success) return DEFAULT; // default;
            m = RegExpNumber.Match(m.Value);
            if (m.Success)
            {
                try
                {
                    return int.Parse(m.Value);
                }
                catch
                {
                    return DEFAULT;
                }
            }
            return DEFAULT;
        }

        // method

        /// <summary>
        /// Find the Y position in a VT cursor position sequence.
        /// This only works if the sequence is a valid position sequence!
        /// </summary>
        /// <param name="escSequence">Valid position sequence</param>
        /// <returns>Y position (column)</returns>
        private static int NewCursorYPosition(string escSequence)
        {
            const int DEFAULT = ScreenYNullCoordinate;
            if (escSequence == null)
                return -1; // error
            Match m = RegExpCursorYPosition.Match(escSequence);
            if (!m.Success) return DEFAULT; // default;
            m = RegExpNumber.Match(m.Value);
            if (m.Success)
            {
                try
                {
                    return int.Parse(m.Value);
                }
                catch
                {
                    return DEFAULT;
                }
            }
            return DEFAULT;
        }
        #endregion

        #region Telnet sub-responses as WILL, WONT ..
        /// <summary>
        /// Add a "WILL" response, e.g. "WILL negotiate about terminal size"
        /// </summary>
        /// <param name="willDoWhat"></param>
        /// <param name="response"></param>
        private static void TelnetWill(byte willDoWhat, MemoryStream response)
        {
            response.WriteByte(TncIac);
            response.WriteByte(TncWill);
            response.WriteByte(willDoWhat);
        }

        /// <summary>
        /// Add a "WONT" response, e.g. "WONT negotiate about terminal size"
        /// </summary>
        /// <param name="wontDoWhat"></param>
        /// <param name="response"></param>
        private static void TelnetWont(byte wontDoWhat, MemoryStream response)
        {
            response.WriteByte(TncIac);
            response.WriteByte(TncWont);
            response.WriteByte(wontDoWhat);
        }

        /// <summary>
        /// Add a "DO" response, e.g. "DO ..."
        /// </summary>
        /// <param name="doWhat"></param>
        /// <param name="response"></param>
        private static void TelnetDo(byte doWhat, MemoryStream response)
        {
            response.WriteByte(TncIac);
            response.WriteByte(TncDo);
            response.WriteByte(doWhat);
        }

        /*
        /// <summary>
        /// Add a "DONT" response, e.g. "DONT ..."
        /// </summary>
        /// <param name="dontDoWhat"></param>
        /// <param name="response"></param>
        private static void TelnetDont(byte dontDoWhat, MemoryStream response)
        {
            response.WriteByte(TncIac);
            response.WriteByte(TncDont);
            response.WriteByte(dontDoWhat);
        }
        */

        /// <summary>
        /// Add a telnet sub-negotiation for ANSI 
        /// terminal
        /// </summary>
        /// <param name="response">MemoryStream</param>
        private static void TelnetSubIsAnsi(MemoryStream response)
        {
            response.WriteByte(TncIac);
            response.WriteByte(TncSb);
            response.WriteByte(TnoTermtype);
            response.WriteByte(TnxIs);
            response.WriteByte(65); // "A"
            response.WriteByte(78); // "N"
            response.WriteByte(83); // "S"
            response.WriteByte(73); // "I"
            response.WriteByte(TncIac);
            response.WriteByte(TncSe);
        }

        // method

        /// <summary>
        /// Telnet sub send terminal size.
        /// </summary>
        /// <param name="w">window width</param>
        /// <param name="h">window height</param>
        /// <param name="response">MemoryStream</param>
        private static void TelnetSubNaws(int w, int h, MemoryStream response)
        {
            var wl = (byte) (0x00FF & w);
            var wh = (byte) (0xFF00 & w);
            var hl = (byte) (0x00FF & h);
            var hh = (byte) (0xFF00 & h);
            response.WriteByte(TncIac);
            response.WriteByte(TncSb);
            response.WriteByte(TnoNaws);
            response.WriteByte(wh);
            response.WriteByte(wl);
            response.WriteByte(hh);
            response.WriteByte(hl);
            response.WriteByte(TncIac);
            response.WriteByte(TncSe);
        }

        // method
        #endregion

        #region Misc. helper-methods
        /// <summary>
        /// Cleans the buffer - not necessary since the values
        /// would just be overwritten - but useful for debugging!
        /// </summary>
        /// <param name="bytesRead">Bytes read and need cleaning</param>
        private void CleanBuffer(int bytesRead)
        {
            if (this._buffer == null) return;
            for (int i = 0; i < bytesRead && i < this._buffer.Length; i++)
            {
                this._buffer[i] = 0;
            }
        }

        // method

        /// <summary>
        /// The MemoryStream bas a bigger byte buffer than bytes
        /// were really written to it. This method fetches all bytes
        /// up the the position written to.
        /// </summary>
        /// <param name="ms">MemoryStream</param>
        /// <returns>really written bytes</returns>
        private static byte[] MemoryStreamToByte(MemoryStream ms)
        {
            // I've tried several options to convert this
            // This one here works but may be improved.
            // ms.Read(wb, 0, wb.Length); did not work
            // ms.ToArray delivers the whole buffer not only the written bytes
            if (ms == null) return null;
            if (ms.Position < 2) return new byte[0];

            // convert
            var wb = new byte[ms.Position];
            byte[] allBytes = ms.ToArray();
            for (int i = 0; i < wb.Length && i < allBytes.Length; i++)
            {
                wb[i] = allBytes[i];
            }
            return wb;
        }

        // method

        /// <summary>
        /// Helper to find a valid IP with a string
        /// </summary>
        /// <param name="candidate">search this string for IP</param>
        /// <returns>IP address or null</returns>
        public static string FindIpAddress(string candidate)
        {
            if (candidate == null) return null;
            Match m = RegExpIp.Match(candidate);
            return m.Success ? m.Value : null;
        }
        #endregion
    } // class

    #region Custom exceptions
    /// <summary>
    /// Exception dealing with connectivity
    /// </summary>
    public class TelnetException : ApplicationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception's message</param>
        public TelnetException(string message) : base(message)
        {
            // further code
        }
    } // Exception class

    /// <summary>
    /// Exception dealing with parsing ...
    /// </summary>
    public class TerminalException : ApplicationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception's message</param>
        public TerminalException(string message) : base(message)
        {
            // further code
        }
    } // Exception class
    #endregion
}