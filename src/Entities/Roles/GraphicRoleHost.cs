using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackCoat.Entities.Roles
{
    public class GraphicRoleHost : Container, IRoleHost
    {
        protected List<Role> _Roles = new List<Role>();


        /// <summary>
        /// Current <see cref="Role"/> that describes the <see cref="IRoleHost"/>s Behavior
        /// </summary>
        public Role CurrentRole => _Roles.LastOrDefault();


        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicRoleHost"/> class.
        /// </summary>
        /// <param name="core">The render core.</param>
        public GraphicRoleHost(Core core) : base(core)
        {
        }


        /// <summary>
        /// Updates the <see cref="IRoleHost"/> using its applied <see cref="Role"/>.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        override public void Update(Single deltaT)
        {
            base.Update(deltaT);
            for (int i = _Roles.Count - 1; i > -1 && _Roles[i].Update(deltaT); i--);
        }

        /// <summary>
        /// Assigns a new <see cref="Role"/> to the <see cref="IRoleHost" /> on top of the current one.
        /// </summary>
        /// <param name="role">The <see cref="Role"/> to assign</param>
        /// <returns>True on success</returns>
        public virtual bool AssignRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            var success = role.Initialize(this);
            if (success) _Roles.Add(role);
            return success;
        }

        /// <summary>
        /// Removes a <see cref="Role" /> from this <see cref="IRoleHost" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /> to remove.</param>
        /// <returns>True on success</returns>
        public virtual bool RemoveRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            var success = _Roles.Remove(role);
            if(success) role.ClearHost();
            return success;
        }
    }
}