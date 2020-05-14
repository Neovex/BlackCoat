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

        public int AdminId { get; }
        public int ServerId { get; }
        public IEnumerable<ServerUser<NetConnection>> ConnectedUsers => _ConnectedClients;


        public ManagedServer(string appIdentifier, Commands<TEnum> commands) : base(appIdentifier)
        {
            _Commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _ConnectedClients = new List<ServerUser<NetConnection>>();
            AdminId = NetIdProvider.NEXT_ID;
            ServerId = NetIdProvider.NEXT_ID;
        }

        // CONTROL

        protected virtual string ValidateName(string name) => String.IsNullOrWhiteSpace(name) ? nameof(NetUser) + NetIdProvider.NEXT_ID : name;

        // INCOMMING

        protected override void NewConnection(NetConnection connection)
        {
            var alias = ValidateName(connection.RemoteHailMessage.ReadString());
            var user = new ServerUser<NetConnection>(_ConnectedClients.Count == 0 ? AdminId : NetIdProvider.NEXT_ID, connection, alias);

            // Accept Client - respond with assigned id, validated alias and server information
            Send(user.Connection, _Commands.Handshake, new Action<NetOutgoingMessage>(m => 
            {
                m.Write(user.Id);
                m.Write(user.Alias);
                m.Write(user.Id == AdminId); // is admin?
                m.Write(ServerId);
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

            UserConnected(user, connection.RemoteHailMessage);
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

        // NOTIFY INHERITANCE

        protected abstract void UserConnected(ServerUser<NetConnection> user, NetIncomingMessage message);
        protected abstract void UserDisconnected(ServerUser<NetConnection> user);
    }
}