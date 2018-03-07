using System;
using BlackCoat.Entities.Roles;

namespace BlackCoat
{
    /// <summary>
    /// Common interface of all <see cref="BlackCoat"/> Entity Types that can be host for a <see cref="Role"/>
    /// </summary>
    public interface IRoleHost
    {
        /// <summary>
        /// Current <see cref="Role"/> that describes the <see cref="IRoleHost"/>s Behavior
        /// </summary>
        Role CurrentRole { get; }

        /// <summary>
        /// Assigns a new <see cref="Role"/> to the <see cref="IRoleHost" /> without removing the current one.
        /// </summary>
        /// <param name="role">The <see cref="Role"/> to assign</param>
        /// <returns>True on success</returns>
        Boolean AssignRole(Role role);

        /// <summary>
        /// Removes a <see cref="Role"/> from this <see cref="IRoleHost" />
        /// </summary>
        /// <param name="role">The <see cref="Role"/> to remove.</param>
        Boolean RemoveRole(Role role);
    }
}