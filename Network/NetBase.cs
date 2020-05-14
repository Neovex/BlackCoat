using System;
using System.Net;
using Lidgren.Network;

namespace BlackCoat.Network
{
    /// <summary>
    /// Base class of the entire network system.
    /// This class handles message processing which is identical for server and client via <see cref="Lidgren"/> Network.
    /// </summary>
    /// <typeparam name="TEnum">The type of the command enumeration for communication.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public abstract class NetBase<TEnum> : IDisposable where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        protected const NetDeliveryMethod _DEFAULT_METHOD = NetDeliveryMethod.ReliableOrdered;

        protected NetPeer _BasePeer;


        public bool Running => _BasePeer == null ? false : _BasePeer.Status == NetPeerStatus.Running;
        public Boolean Disposed { get; private set; }


        public NetBase(NetPeer peer)
        {
            _BasePeer = peer ?? throw new ArgumentNullException(nameof(peer));
        }
        ~NetBase()
        {
            Dispose(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool safeToDisposeManaged)
        {
            if (Disposed) return;
            Disposed = true;
            if (safeToDisposeManaged)
            {
                _BasePeer.Shutdown("Unexpected Shutdown");
            }
            _BasePeer = null;
        }

        public void ProcessMessages()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(NetBase<TEnum>));

            NetIncomingMessage msg;
            while ((msg = _BasePeer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Error: // Should never happen
                        Log.Fatal(msg.MessageType, msg.SenderConnection.Status);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        switch (msg.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Connected:
                                NewConnection(msg.SenderConnection);
                                break;

                            case NetConnectionStatus.Disconnected:
                                ConnectionLost(msg.SenderConnection);
                                break;
                        }
                        break;

                    case NetIncomingMessageType.Data:
                        if (_BasePeer.Status == NetPeerStatus.Running) ProcessIncommingData((TEnum)(Object)msg.ReadInt32(), msg);
                        break;

                    case NetIncomingMessageType.DiscoveryRequest:
                        var response = _BasePeer.CreateMessage();
                        if (HandleDiscoveryRequest(response))
                        {
                            _BasePeer.SendDiscoveryResponse(response, msg.SenderEndPoint);
                        }
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        DiscoveryResponseReceived(msg);
                        break;

                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                        Log.Debug(msg.ReadString());
                        break;

                    case NetIncomingMessageType.WarningMessage:
                        Log.Warning(msg.ReadString());
                        break;

                    case NetIncomingMessageType.ErrorMessage:
                        Log.Error(msg.ReadString());
                        break;

                    case NetIncomingMessageType.NatIntroductionSuccess:
                        Log.Info(msg.MessageType);
                        break;

                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        LatencyUpdateReceived(msg.SenderConnection, msg.ReadFloat());
                        break;
                }
                _BasePeer.Recycle(msg);
            }
        }


        // INCOMMING

        protected abstract void ProcessIncommingData(TEnum subType, NetIncomingMessage msg);

        protected abstract void NewConnection(NetConnection senderConnection);

        protected abstract void ConnectionLost(NetConnection senderConnection);

        protected abstract bool HandleDiscoveryRequest(NetOutgoingMessage msg);
        protected abstract void DiscoveryResponseReceived(NetIncomingMessage msg);
        protected abstract void LatencyUpdateReceived(NetConnection senderConnection, float latency);


        // OUTGOING

        protected virtual NetOutgoingMessage CreateMessage(TEnum subType, Action<NetOutgoingMessage> operation = null)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(NetBase<TEnum>));
            var message = _BasePeer.CreateMessage();
            message.Write(subType.ToInt32(Format.DEFAULT));
            operation?.Invoke(message);
            return message;
        }

        
        //MISC
        private sealed class Format : IFormatProvider
        {
            // SINGLETON
            public static readonly Format DEFAULT = new Format();

            private Format()
            { }

            public object GetFormat(Type formatType) => typeof(int);
        }
    }
}