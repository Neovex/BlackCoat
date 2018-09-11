using System;

namespace BlackCoat.Network
{
    /// <summary>
    /// Represents user that is connected to the server.
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    /// <seealso cref="BlackCoat.Network.NetUser" />
    public class ServerUser<TChannel> : NetUser where TChannel : class
    {
        /// <summary>
        /// The communication channel to the user.
        /// </summary>
        public TChannel Connection { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerUser{TChannel}"/> class.
        /// </summary>
        /// <param name="id">The UID.</param>
        /// <param name="channel">The communication channel.</param>
        /// <param name="alias">The user alias.</param>
        /// <exception cref="ArgumentNullException">channel</exception>
        internal ServerUser(Int32 id, TChannel channel, String alias) : base(id, alias)
        {
            Connection = channel ?? throw new ArgumentNullException(nameof(channel));
        }
    }
}