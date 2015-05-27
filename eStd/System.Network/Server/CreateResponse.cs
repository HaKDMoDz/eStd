using System.IO;
using System.Net.Sockets;
using System.Text;
using Microsoft.Win32;

namespace System.Network.Server
{
    public class CreateResponse
    {
        RegistryKey registryKey = Registry.ClassesRoot;
        public Socket ClientSocket = null;
        private Encoding _charEncoder = Encoding.UTF8;
        private string _contentPath ;
        public Filehandler Filehandler;

        public CreateResponse(Socket clientSocket,string contentPath)
        {
            _contentPath = contentPath;
            ClientSocket = clientSocket;
            Filehandler=new Filehandler(_contentPath);
        }

        public void RequestUrl(string requestedFile)
        {
            int dotIndex = requestedFile.LastIndexOf('.') + 1;
            if (dotIndex > 0)
            {
                if (Filehandler.DoesFileExists(requestedFile))    //If yes check existence of the file
                    SendResponse(ClientSocket, Filehandler.ReadFile(requestedFile), "200 Ok", GetTypeOfFile(registryKey, (_contentPath + requestedFile)));
                    else
                        SendErrorResponce(ClientSocket);      // We don't support this extension.
            }
            else   //find default file as index .htm of index.html
            {
                if (Filehandler.DoesFileExists("\\index.htm"))
                    SendResponse(ClientSocket, Filehandler.ReadFile("\\index.htm"), "200 Ok", "text/html");
                else if (Filehandler.DoesFileExists("\\index.html"))
                    SendResponse(ClientSocket, Filehandler.ReadFile("\\index.html"), "200 Ok", "text/html");
                else
                    SendErrorResponce(ClientSocket);
            }
        }

        private string GetTypeOfFile(RegistryKey registryKey,string fileName)
        {
            RegistryKey fileClass = registryKey.OpenSubKey(Path.GetExtension(fileName));
            return fileClass.GetValue("Content Type").ToString();
        }

        private void SendErrorResponce(Socket clientSocket)
        {
            SendResponse(clientSocket, null, "404 Not Found", "text/html");
        }


        private void SendResponse(Socket clientSocket, byte[] byteContent, string responseCode, string contentType)
        {
            try
            {
                byte[] byteHeader = CreateHeader(responseCode, byteContent.Length, contentType);
                clientSocket.Send(byteHeader);
                clientSocket.Send(byteContent);
                
                clientSocket.Close();
            }
            catch
            {
            }
        }

        private byte[] CreateHeader(string responseCode, int contentLength, string contentType)
        {
            return _charEncoder.GetBytes("HTTP/1.1 " + responseCode + "\r\n"
                                  + "Server: Simple Web Server\r\n"
                                  + "Content-Length: " + contentLength + "\r\n"
                                  + "Connection: close\r\n"
                                  + "Content-Type: " + contentType + "\r\n\r\n");
        }
    }
}
