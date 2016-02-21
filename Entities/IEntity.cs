using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace BlackCoat.Entities
{
    /// <summary>
    /// Shared interface of all supported Entity Types
    /// </summary>
    public interface IEntity:Drawable
    {
        // Properties ######################################################################
        /// <summary>
        /// Location of the Entity within its parent container
        /// </summary>
        Vector2f Position { get; set; }

        /// <summary>
        /// Parent Container of this Entity
        /// </summary>
        Container Parent { get; set; }

        /// <summary>
        /// Determines the Visisbility of the Entity
        /// </summary>
        Boolean Visible { get; set; }

        /// <summary>
        /// Target Render View
        /// </summary>
        View View { get; set; }

        /// <summary>
        /// Renderstate of the entity
        /// </summary>
        RenderStates RenderState { get; set; }

        /// <summary>
        /// Blending method used for Rendering
        /// </summary>
        BlendMode BlendMode { get; set; }

        /// <summary>
        /// Gobal Color/Tint of the Entitiy
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Alpha Value
        /// </summary>
        Single Alpha { get; set; }

        /// <summary>
        /// Current Role that descripes the Entities Behaviour
        /// </summary>
        Role CurrentRole { get; }


        // Methods ######################################################################
        /// <summary>
        /// Updates the current Entity using its applied Role.
        /// Can be overriden by derived classes.
        /// </summary>
        /// <param name="gameTime">Current gametime</param>
        void Update(Single deltaT);

        /// <summary>
        /// Renders the Entity into the scene
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