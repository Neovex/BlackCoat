﻿using System;
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
        public Boolean IsConnected => _BasePeer.ConnectionsCount != 0;


        public Client(String appId) : base(new NetClient(new NetPeerConfiguration(appId)))
        {
            _Client = _BasePeer as NetClient;
        }


        public void Connect(String host, Int32 port, String hail)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Client<TEnum>));
            if (String.IsNullOrWhiteSpace(host)) throw new ArgumentException(nameof(host));

            _Client.Start();
            _Client.Connect(host, port, _Client.CreateMessage(hail));
        }

        public void Disconnect(string disconnectMessage)
        {
            _Client.Disconnect(disconnectMessage);
            // TODO : check if Disconnected needs to be called extra here
        }


        // OVERRIDES (to hide the underlying NetConnection)

        protected override void NewConnection(NetConnection senderConnection) => Connected();
        protected override void ConnectionLost(NetConnection senderConnection) => Disconnected();

        protected abstract void Connected();
        protected abstract void Disconnected();
        

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