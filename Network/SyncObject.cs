using System;
using Lidgren.Network;

namespace BlackCoat.Network
{
    /// <summary>
    /// Abstract base class for objects that need to be synchronized between client and server.
    /// </summary>
    public abstract class SyncObject
    {
        /// <summary>
        /// Net ID
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Determines whether this object has unsynchronized changes when properly set by inherited classes.
        /// </summary>
        public Boolean IsDirty { get; protected set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="SyncObject"/> class.
        /// </summary>
        /// <param name="id">The net identifier.</param>
        public SyncObject(int id) => Id = id;
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncObject"/> class.
        /// </summary>
        /// <param name="id">The net identifier.</param>
        /// <param name="message">The incoming message to create this <see cref="SyncObject"/> from.</param>
        public SyncObject(int id, NetIncomingMessage message) : this(id) => Deserialize(message);


        /// <summary>
        /// Serializes the <see cref="SyncObject"/> into a <see cref="NetOutgoingMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="NetOutgoingMessage"/> to write the <see cref="SyncObject"/> to</param>
        public void Serialize(NetOutgoingMessage message)
        {
            message.Write(Id);
            SerializeInternal(message);
            IsDirty = false;
        }

        /// <summary>
        /// Serializes the all additional fields into a <see cref="NetOutgoingMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="NetOutgoingMessage"/> to write the additional fields to</param>
        protected abstract void SerializeInternal(NetOutgoingMessage message);

        /// <summary>
        /// De-serializes the all additional fields from a <see cref="NetIncomingMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="NetIncomingMessage"/> to read the additional field values from</param>
        public abstract void Deserialize(NetIncomingMessage message);
    }
}