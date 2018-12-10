using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;

namespace BlackCoat.Network
{
    /// <summary>
    /// Abstract server implementation. Adds management for connection verification and connected users.
    /// </summary>
    /// <typeparam name="TEnum">The type of the command enumeration for communication.</typeparam>
    /// <seealso cref="Network.Server{TEnum}" />
    public abstract class ManagedServer<TEnum> : Server<TEnum> where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        protected Commands<TEnum> _Commands;
        protected List<ServerUser<NetConnection>> _ConnectedClients;

        public abstract int AdminId { get; }
        public abstract int NextClientId { get; }
        public virtual IEnumerable<ServerUser<NetConnection>> ConnectedUsers => _ConnectedClients;


        public ManagedServer(string appIdentifier, Commands<TEnum> commands) : base(appIdentifier)
        {
            _Commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _ConnectedClients = new List<ServerUser<NetConnection>>();
        }

        // CONTROL

        protected virtual int GetNextFreeClientID() => _ConnectedClients.Count == 0 ? AdminId : NextClientId;

        protected virtual string ValidateName(string name) => name;

        // INCOMMING

        protected override void NewConnection(NetConnection connection)
        {
            var user = new ServerUser<NetConnection>(GetNextFreeClientID(), connection, ValidateName(connection.RemoteHailMessage.ReadString()));

            // Accept Client - respond with assigned id, validated alias and the list of other connected users
            Send(user.Connection, _Commands.Handshake, new Action<NetOutgoingMessage>(m => 
            {
                m.Write(user.Id);
                m.Write(user.Alias);
                m.Write(_ConnectedClients.Count);
                foreach (var client in _ConnectedClients)
                {
                    m.Write(client.Id);
                    m.Write(client.Alias);
                }
            }));

            // Add user to user management list
            _ConnectedClients.Add(user);
            // Inform other Clients of new user
            Broadcast(_Commands.UserConnected, new Action<NetOutgoingMessage>(m => { m.Write(user.Id); m.Write(user.Alias); }));

            UserConnected(user);
        }


        protected override void ConnectionLost(NetConnection connection)
        {
            var user = _ConnectedClients.FirstOrDefault(u => u.Connection == connection);
            if (user == null)
            {
                Log.Warning("Received disconnect message from unknown connection:", connection.RemoteEndPoint);
            }
            else
            {
                _ConnectedClients.Remove(user);
                Broadcast(_Commands.UserDisconnected, m => m.Write(user.Id));
                UserDisconnected(user);
            }
        }

        protected override void LatencyUpdateReceived(NetConnection connection, float latency)
        {
            var user = _ConnectedClients.FirstOrDefault(u => u.Connection == connection);
            if (user == null)
            {
                Log.Warning("Received latency from unknown connection:", connection.RemoteEndPoint);
            }
            else
            {
                user.Latency = (int)(latency * 1000);
            }
        }

        protected abstract void UserConnected(ServerUser<NetConnection> user);
        protected abstract void UserDisconnected(ServerUser<NetConnection> user);
    }
}