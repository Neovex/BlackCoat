using System;

namespace BlackCoat
{
    public abstract class Role
    {
#region Variables #####################
        protected Core _Core;
        protected IEntity _Target;
        protected Boolean _IsInitialized;
#endregion


#region Properties ####################
        /// <summary>
        /// Determines if the Role is initialized and ready to use
        /// </summary>
        public virtual Boolean IsInitialized { get { return _IsInitialized; } }

        /// <summary>
        /// The Entity of wich the Role is currently applied to
        /// </summary>
        public virtual IEntity Target
        {
            get { return _Target; }
            internal set { _Target = value; }
        }
#endregion


#region CTOR ##########################
        /// <summary>
        /// Abstract base constructor - initializes the Core
        /// </summary>
        /// <param name="core">BlackCoat Core</param>
        public Role(Core core)
        {
            if (core == null) throw new ArgumentNullException("core");
            _Core = core;
        }
#endregion


#region Methods #######################
        /// <summary>
        /// Initializes the Role
        /// </summary>
        /// <returns>True on success</returns>
        public Boolean Initialize()
        {
            _IsInitialized = DoInitialize();
            return _IsInitialized;
        }

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
#endregion
    }
}