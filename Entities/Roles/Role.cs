using System;

namespace BlackCoat.Entities.Roles
{
    /// <summary>
    /// Base class of all Entity Roles
    /// </summary>
    public abstract class Role
    {
        // Variables #######################################################################
        protected Core _Core;


        // Properties ######################################################################
        /// <summary>
        /// Determines if the Role is initialized and ready to use
        /// </summary>
        public virtual Boolean IsInitialized { get; private set; }

        /// <summary>
        /// The Entity the Role is currently assigned to
        /// </summary>
        public virtual IEntity Host { get; private set; }


        // CTOR ############################################################################
        /// <summary>
        /// Abstract base constructor - initializes the Core
        /// </summary>
        /// <param name="core">BlackCoat Core</param>
        public Role(Core core) => _Core = core ?? throw new ArgumentNullException(nameof(core));


        // Methods #########################################################################
        /// <summary>
        /// Initializes the Role
        /// </summary>
        /// <param name="host">The entity this role was assigned to.</param>
        public bool Initialize(IEntity host)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host));
            return IsInitialized = InitializeRole();
        }

        /// <summary>
        /// Implementation of Role initialization
        /// </summary>
        /// <returns>True on success</returns>
        protected abstract bool InitializeRole();

        /// <summary>
        /// Updates the Target Entity using this Role.
        /// </summary>
        /// <param name="deltaT">Frame time</param>
        /// <returns>If the Update shall be passed to underlying Roles</returns>
        public abstract Boolean Update(float deltaT);

        /// <summary>
        /// Clears the host.
        /// </summary>
        internal void ClearHost() => Host = null;
    }
}