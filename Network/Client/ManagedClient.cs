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


        public int ServerId { get; private set; }
        public Int32 Id { get; private set; }
        public String Alias { get; private set; }
        public Boolean IsAdmin { get; private set; }
        public Boolean Validated { get; private set; }
        public IEnumerable<NetUser> ConnectedUsers => _ConnectedClients;


        public ManagedClient(String appId, String alias, Commands<TEnum> commands) : base(appId)
        {
            Alias = !String.IsNullOrWhiteSpace(alias) ? alias : throw new ArgumentException(nameof(alias));
            _Commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _ConnectedClients = new List<NetUser>();
        }

        public bool Connect(String host, Int32 port)
        {
            return Connect(host, port, Alias);
        }
        public bool Connect(IPEndPoint host)
        {
            return Connect(host, Alias);
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
            IsAdmin = msg.ReadBoolean();
            ServerId = msg.ReadInt32();
            var clientCount = msg.ReadInt32();
            for (int i = 0; i < clientCount; i++)
            {
                HandleUserConnected(msg);
            }
            ConnectionValidated(Id, Alias);
            Validated = true;
        }

        private void HandleUserConnected(NetIncomingMessage msg)
        {
            var id = msg.ReadInt32();
            if (id == Id) return; // Ignore our own join notification - we already know we are connected
            var user = new NetUser(id, msg.ReadString());
            _ConnectedClients.Add(user);
            UserConnected(user);
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
                _ConnectedClients.Remove(user);
                UserDisconnected(user);
            }
        }

        protected abstract void ConnectionValidated(int id, string alias);
        protected abstract void UserConnected(NetUser user);
        protected abstract void UserDisconnected(NetUser user);
        protected abstract void DataReceived(TEnum subType, NetIncomingMessage msg); // hmm msg UU mit interface ersetzen

        protected override void Connected()
        {
            // Intentionally empty since this logic is replaced by -> ConnectionValidated
        }
    }
}