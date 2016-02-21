using System;
using BlackCoat.Entities;

namespace BlackCoat
{
    /// <summary>
    /// Baseclass of all Entity Roles
    /// </summary>
    public abstract class Role
    {
        // Variables #######################################################################
        protected Core _Core;


        // Properties ######################################################################
        /// <summary>
        /// Determines if the Role is initialized and ready to use
        /// </summary>
        public virtual Boolean IsInitialized { get; protected set; }

        /// <summary>
        /// The Entity of wich the Role is currently applied to
        /// </summary>
        public virtual IEntity Target { get; internal set; }


        // CTOR ############################################################################
        /// <summary>
        /// Abstract base constructor - initializes the Core
        /// </summary>
        /// <param name="core">BlackCoat Core</param>
        public Role(Core core)
        {
            if (core == null) throw new ArgumentNullException("core");
            _Core = core;
        }


        // Methods #########################################################################
        /// <summary>
        /// Initializes the Role
        /// </summary>
        /// <returns>True on success</returns>
        public Boolean Initialize() { return IsInitialized = DoInitialize(); }

        /// <summary>
        /// Initializes the Role internally
        /// </summary>
        /// <returns>True on success</returns>
        protected abstract Boolean DoInitialize();

        /// <summary>
        /// Updates the Target Entity using this Role.
        /// </summary>
        /// <param name="deltaT">Frametime</param>
        /// <returns>If the Update shall be passed to underlying Roles</returns>
        public abstract Boolean Update(float deltaT);
    }
}