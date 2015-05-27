using System;
using System.Net;
using System.Net.Sockets;
using System.Network.Server;
using System.Text;

namespace System.Network.Server
{
    public class WebServer
    {
        // check for already running
        private Encoding _charEncoder = Encoding.UTF8;

        // Directory to host our contents
        private string _contentPath;
        private bool _running;
        private Socket _serverSocket;
        private int _timeout = 5;

        //create socket and initialization
        private void InitializeSocket(IPAddress ipAddress, int port, string contentPath) //create socket
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(ipAddress, port));
            _serverSocket.Listen(10); //no of request in queue
            _serverSocket.ReceiveTimeout = _timeout;
            _serverSocket.SendTimeout = _timeout;
            _running = true; //socket created
            _contentPath = contentPath;
        }


        public void Start(IPAddress ipAddress, int port, string contentPath)
        {
            try
            {
                InitializeSocket(ipAddress, port, contentPath);
            }
            catch
            {
                Console.WriteLine("Error in creating server socker");
                Console.ReadLine();
            }
            while (_running)
            {
                var requestHandler = new RequestHandler(_serverSocket, contentPath);
                requestHandler.acceptRequest();
            }
        }


        public void Stop()
        {
            _running = false;
            try
            {
                _serverSocket.Close();
            }
            catch
            {
                Console.WriteLine("Error in closing server or server already closed");
                Console.ReadLine();
            }
            _serverSocket = null;
        }
    }
}