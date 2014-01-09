using System;
using SFML.Graphics;
using SFML.Window;

namespace BlackCoat
{
    public interface IEntity
    {
        // Properties ######################################################################
        /// <summary>
        /// Parent Container of this Entity
        /// </summary>
        Container Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Determines the Visisbility of the Entity
        /// </summary>
        Boolean Visible
        {
            get;
            set;
        }

        /// <summary>
        /// Alpha Value
        /// </summary>
        Single Alpha
        {
            get;
            set;
        }

        /// <summary>
        /// Current Role that descripes the Entities Behaviour
        /// </summary>
        Role CurrentRole { get; }


        // Methods ######################################################################
        /// <summary>
        /// Updates the Current Entity using its applied Role.
        /// Can be overriden by derived classes.
        /// </summary>
        /// <param name="gameTime">Current gametime</param>
        void Update(Single deltaT);

        /// <summary>
        /// Draws the Graphic of the Entity if it is visible
        /// </summary>
        void Draw();


        // Roles ########################################################################
        /// <summary>
        /// Assigns a new Role to the Entity without removing the current one.
        /// </summary>
        /// <param name="role">The Role to assign</param>
        /// <param name="supressInitialization">Supress initialization call on assigned role</param>
        void AssignRole(Role role, Boolean supressInitialization = false);

        /// <summary>
        /// Assigns a new Role to the Entity after removing the current one.
        /// </summary>
        /// <param name="role">The Role to assign</param>
        /// <param name="supressInitialization">Supress initialization call on assigned role</param>
        /// <returns>The removed role if there was one - otherwhise null</returns>
        Role ReplaceRole(Role role, Boolean supressInitialization = false);

        /// <summary>
        /// Removes the currently active Role from this Entity
        /// </summary>
        /// <returns>The removed role if there was one - otherwhise null</returns>
        Role RemoveRole();
    }
}