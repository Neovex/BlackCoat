using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Lidgren.Network;

namespace BlackCoat.Network
{
    /// <summary>
    /// Abstract server implementation. Handles hosting and broadcasting logic for inherited classes.
    /// </summary>
    /// <typeparam name="TEnum">The type of the command enumeration for communication.</typeparam>
    /// <seealso cref="Network.NetBase{TEnum}" />
    public abstract class Server<TEnum> : NetBase<TEnum> where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        private NetServer _Server;

        public String Name { get; private set; }
        public String AppIdentifier { get; private set; }


        public Server(string appIdentifier):base(new NetPeer(new NetPeerConfiguration(appIdentifier)))
        {
            if (String.IsNullOrWhiteSpace(appIdentifier)) throw new ArgumentException(nameof(appIdentifier));
            AppIdentifier = appIdentifier;
        }


        // CONTROL

        public void Host(string name, int port)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Server<TEnum>));
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            Name = name;

            StopServer(String.Empty);

            var config = new NetPeerConfiguration(AppIdentifier);
            config.Port = port;
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            _BasePeer = _Server = new NetServer(config);
            _Server.Start();
            Log.Info("Server started, listening on:", port);
        }

        public void StopServer(string stopMessage = "")
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Server<TEnum>));
            if (_BasePeer.Status != NetPeerStatus.Running) return;
            _BasePeer.Shutdown(stopMessage);
            BlockUntilShutdownIsComplete();
            Log.Info("Server Stopped");
        }

        /// <summary>
        /// Blocks the execution until BasePeer shutdown is complete.
        /// </summary>
        private void BlockUntilShutdownIsComplete()
        {
            /*
            Hacky as fuck, i know - but its working flawlessly
            we wait until the old serving thread has finished - this is practically a thread join
            Does not last longer than a couple milliseconds
            */
            var c = 0;
            while (_BasePeer.Status == NetPeerStatus.ShutdownRequested)
            {
                c++;
                Thread.Sleep(1);
            }
            Log.Debug("Shutdown took ~", c, "milliseconds");
        }

        // INCOMMING
        protected override void DiscoveryResponseReceived(NetIncomingMessage msg) { }

        // OUTGOING
        protected virtual void Broadcast(TEnum subType, Action<NetOutgoingMessage> operation = null, NetDeliveryMethod netMethod = _DEFAULT_METHOD)
        {
            Broadcast(CreateMessage(subType, operation), netMethod);
        }
        protected virtual void Broadcast(NetOutgoingMessage message, NetDeliveryMethod netMethod = _DEFAULT_METHOD)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Server<TEnum>));
            _Server.SendToAll(message, netMethod);
        }

        protected virtual void Send(NetConnection receiver, TEnum subType, Action<NetOutgoingMessage> operation = null, NetDeliveryMethod netMethod = _DEFAULT_METHOD)
        {
            Send(CreateMessage(subType, operation), receiver, netMethod);
        }
        protected virtual void Send(NetOutgoingMessage message, NetConnection receiver, NetDeliveryMethod netMethod = _DEFAULT_METHOD)
        {
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            if (Disposed) throw new ObjectDisposedException(nameof(Server<TEnum>));
            _Server.SendMessage(message, receiver, netMethod);
        }
    }
}