using System.Communication.Communication.Scs.Client;
using System.Communication.Communication.Scs.Communication.Messengers;
using Hik.Communication.Scs.Communication;

namespace System.Communication.Communication.Scs.Client
{
    /// <summary>
    /// Represents a client to connect to server.
    /// </summary>
    public interface IScsClient : IMessenger, IConnectableClient
    {
        //Does not define any additional member
    }
}
