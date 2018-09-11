using System;

namespace BlackCoat.Network
{
    /// <summary>
    /// Represents a connected user
    /// </summary>
    public class NetUser
    {
        /// <summary>
        /// Unique user id.
        /// </summary>
        public Int32 Id { get; private set; }

        /// <summary>
        /// User alias.
        /// </summary>
        public String Alias { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetUser"/> class.
        /// </summary>
        /// <param name="id">The UID.</param>
        /// <param name="alias">The user alias.</param>
        internal NetUser(Int32 id, String alias)
        {
            Id = id;
            Alias = alias;
        }
    }
}