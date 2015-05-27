using System;
using System.Net.Sockets;
using System.Network.Server;
using System.Text;
using System.Threading;

namespace System.Network.Server
{
    class RequestHandler
    {
        private Socket _serverSocket;
        private int _timeout;
        private string _contentPath;
        private Encoding _charEncoder = Encoding.UTF8;

        public RequestHandler(Socket serverSocket, String contentPath)
        {
            _serverSocket = serverSocket;
            _timeout = 5;
            _contentPath = contentPath;
        }

        public void acceptRequest()
        {
            Socket clientSocket = null;
            try
            {
                // Create new thread to handle the request and continue to listen the socket.
                clientSocket = _serverSocket.Accept();

                var requestHandler = new Thread(() =>
                {
                    clientSocket.ReceiveTimeout = _timeout;
                    clientSocket.SendTimeout = _timeout;
                    HandleTheRequest(clientSocket);
                });
                requestHandler.Start();
            }
            catch
            {
                Console.WriteLine("Error in accepting client request");
                Console.ReadLine();
                if (clientSocket != null)
                    clientSocket.Close();
            }
        }

        private void HandleTheRequest(Socket clientSocket)
        {
            var requestParser = new RequestParser();
            string requestString = DecodeRequest(clientSocket);
            requestParser.Parser(requestString);

            if (requestParser.HttpMethod.Equals("get", StringComparison.InvariantCultureIgnoreCase))
            {
                var createResponse = new CreateResponse(clientSocket, _contentPath);
                createResponse.RequestUrl(requestParser.HttpUrl);
            }
            else
            {
                Console.WriteLine("unemplimented mothode");
                Console.ReadLine();
            }
            StopClientSocket(clientSocket);
        }

        public void StopClientSocket(Socket clientSocket)
        {
            if (clientSocket != null)
                clientSocket.Close();
        }

        private string DecodeRequest(Socket clientSocket)
        {
            var receivedBufferlen = 0;
            var buffer = new byte[10240];
            try
            {
              //  clientSocket.Receive(buffer);
                receivedBufferlen = clientSocket.Receive(buffer);
            }
            catch (Exception)
            {
                //Console.WriteLine("buffer full");
                Console.ReadLine();
            }
            return _charEncoder.GetString(buffer, 0, receivedBufferlen);
        }
    }
}
