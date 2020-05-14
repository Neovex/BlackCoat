using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Lidgren.Network;

namespace BlackCoat.Network
{
    /// <summary>
    /// Abstract client implementation. Handles connect and disconnect logic for inherited classes.
    /// </summary>
    /// <typeparam name="TEnum">The type of the command enumeration for communication.</typeparam>
    /// <seealso cref="Network.NetBase{TEnum}" />
    public abstract class Client<TEnum> : NetBase<TEnum> where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        private NetClient _Client;


        public Boolean IsConnected => _Client.ConnectionsCount != 0;
        public int Latency { get; private set; }
        public string LastError { get; private set; }


        public Client(String appId) : base(new NetClient(new NetPeerConfiguration(appId)))
        {
            _Client = _BasePeer as NetClient;
            _Client.Configuration.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            _Client.Configuration.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
        }


        public virtual bool Connect(String host, Int32 port, Action<NetOutgoingMessage> connectMessage = null)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Client<TEnum>));
            if (String.IsNullOrWhiteSpace(host)) throw new ArgumentException(nameof(host));
            try
            {
                _Client.Start();
                if (connectMessage == null)
                {
                    _Client.Connect(host, port);
                }
                else
                {
                    var message = _Client.CreateMessage();
                    connectMessage.Invoke(message);
                    _Client.Connect(host, port, message);
                }
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Log.Error(ex);
                return false;
            }
        }
        public virtual bool Connect(IPEndPoint host, Action<NetOutgoingMessage> connectMessage = null)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Client<TEnum>));
            if (host == null) throw new ArgumentNullException(nameof(host));
            try
            {
                _Client.Start();
                if (connectMessage == null)
                {
                    _Client.Connect(host);
                }
                else
                {
                    var message = _Client.CreateMessage();
                    connectMessage.Invoke(message);
                    _Client.Connect(host, message);
                }
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Log.Error(ex);
                return false;
            }
        }

        public void Disconnect(string disconnectMessage)
        {
            if (!IsConnected) return;
            _Client.Disconnect(disconnectMessage);
            Disconnected();
        }


        // OVERRIDES (to hide the underlying NetConnection)

        protected override bool HandleDiscoveryRequest(NetOutgoingMessage msg) => false;

        protected override void NewConnection(NetConnection senderConnection) => Connected();
        protected override void ConnectionLost(NetConnection senderConnection) => Disconnected();

        protected abstract void Connected();
        protected abstract void Disconnected();

        // INCOMMING
        protected override void LatencyUpdateReceived(NetConnection senderConnection, float latency)
        {
            Latency = (int)(latency * 1000);
        }


        // OUTGOING
        protected virtual void Send(TEnum subType, Action<NetOutgoingMessage> operation = null, NetDeliveryMethod netMethod = _DEFAULT_METHOD)
        {
            Send(CreateMessage(subType, operation), netMethod);
        }
        protected virtual void Send(NetOutgoingMessage message, NetDeliveryMethod netMethod = _DEFAULT_METHOD)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Client<TEnum>));
            _Client.SendMessage(message, netMethod);
        }
    }
}