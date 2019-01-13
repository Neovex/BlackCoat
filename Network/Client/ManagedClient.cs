using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Lidgren.Network;

namespace BlackCoat.Network
{
    /// <summary>
    /// Abstract client implementation. Adds management for connection verification and connected users.
    /// </summary>
    /// <typeparam name="TEnum">The type of the command enumeration for communication.</typeparam>
    /// <seealso cref="BlackCoat.Network.Client{TEnum}" />
    public abstract class ManagedClient<TEnum> : Client<TEnum> where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        protected Commands<TEnum> _Commands;
        protected List<NetUser> _ConnectedClients;


        public virtual Int32 Id { get; protected set; }
        public virtual String Alias { get; protected set; }
        public virtual Boolean Validated { get; protected set; }
        public virtual IEnumerable<NetUser> ConnectedUsers { get { return _ConnectedClients; } }
        public virtual Boolean IsAdmin { get { return Id == AdminId; } }
        public abstract Int32 AdminId { get; }


        public ManagedClient(String appId, String alias, Commands<TEnum> commands) : base(appId)
        {
            Alias = !String.IsNullOrWhiteSpace(alias) ? alias : throw new ArgumentException(nameof(alias));
            _Commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _ConnectedClients = new List<NetUser>();
        }

        public void Connect(String host, Int32 port)
        {
            Connect(host, port, Alias);
        }
        public void Connect(IPEndPoint host)
        {
            Connect(host, Alias);
        }

        protected override void ProcessIncommingData(TEnum subType, NetIncomingMessage msg)
        {
            if (Validated) // process messages normally
            {
                if (subType.Equals(_Commands.UserConnected))
                {
                    HandleUserConnected(msg);
                }
                else if (subType.Equals(_Commands.UserDisconnected))
                {
                    HandleUserDisconnected(msg);
                }
                else
                {
                    DataReceived(subType, msg);
                }
            }
            else if (subType.Equals(_Commands.Handshake)) // only listen to handshakes
            {
                HandleHandshake(msg);
            }
        }

        private void HandleHandshake(NetIncomingMessage msg)
        {
            Id = msg.ReadInt32();
            Alias = msg.ReadString();
            var clientCount = msg.ReadInt32();
            for (int i = 0; i < clientCount; i++)
            {
                HandleUserConnected(msg, false);
            }
            ConnectionValidated(Id, Alias);
            Validated = true;
        }

        private void HandleUserConnected(NetIncomingMessage msg, bool callUserConnected = true)
        {
            var id = msg.ReadInt32();
            if (id != Id)
            {
                var user = new NetUser(id, msg.ReadString());
                _ConnectedClients.Add(user);
                if (callUserConnected) UserConnected(user);
            }
        }

        private void HandleUserDisconnected(NetIncomingMessage msg)
        {
            var id = msg.ReadInt32();
            var user = _ConnectedClients.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                Log.Warning("Received disconnect from unknown user id", id);
            }
            else
            {
                UserDisconnected(user);
            }
        }

        protected abstract void ConnectionValidated(int id, string alias);
        protected abstract void UserConnected(NetUser user);
        protected abstract void UserDisconnected(NetUser user);
        protected abstract void DataReceived(TEnum subType, NetIncomingMessage msg); // hmm msg UU mit interface ersetzen

        protected override void Disconnected()
        {
            Validated = false;
        }
    }
}