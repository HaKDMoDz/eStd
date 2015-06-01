using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace System.Extensions
{
    /// <summary>
    /// Contains various useful networking-related extensions.
    /// </summary>
    public static class NetworkingExtensions
    {
        /// <summary>
        /// Sends a file to a remote computer using sockets.  Requires a waiting server at the specified IP and port.
        /// </summary>
        /// <param name="fileInfo">Local file to send.</param>
        /// <param name="remoteIP">Remote IP address.</param>
        /// <param name="remotePort">Remote port.</param>
        /// <example>
        ///     <code language="c#">
        ///         FileInfo fi = new FileInfo(@"C:\file.jpg");
        ///         fi.SendToComputer(address, port);
        ///     </code>
        /// </example>
        public static void SendToComputer(this FileInfo fileInfo, string remoteIP, int remotePort)
        {
            IPAddress address = IPAddress.Parse(remoteIP);

            IPEndPoint endpoint = new IPEndPoint(address, remotePort);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SendBufferSize = 1048576;
            socket.Connect(endpoint);

            FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fs);

            while (fs.Position < fs.Length - 1)
            {
                if (socket != null)
                {
                    byte[] bytes;

                    if (fs.Length - fs.Position < 1048576)
                    {
                        int remainder = Convert.ToInt32(fs.Length - fs.Position - 1);

                        bytes = br.ReadBytes(remainder);
                    }
                    else
                    {
                        bytes = br.ReadBytes(1048576);
                    }

                    socket.Send(bytes);
                }
            }

            br.Close();

            fs.Close();
        }
    }
}
