using System;
using SFML.Graphics;
using BlackCoat.Entities;


namespace BlackCoat
{
    /// <summary>
    /// Common interface of all <see cref="BlackCoat"/> Entity Types
    /// </summary>
    public interface IEntity:Drawable, ITransformable
    {
        // Properties ######################################################################
        /// <summary>
        /// Parent Container of the <see cref="IEntity"/>
        /// </summary>
        Container Parent { get; set; }

        /// <summary>
        /// Determines the Visisbility of the <see cref="IEntity"/>
        /// </summary>
        Boolean Visible { get; set; }

        /// <summary>
        /// Target Render View
        /// </summary>
        View View { get; set; }

        /// <summary>
        /// Renderstate of the <see cref="IEntity"/>
        /// </summary>
        RenderStates RenderState { get; set; }

        /// <summary>
        /// Blending method used for Rendering
        /// </summary>
        BlendMode BlendMode { get; set; }

        /// <summary>
        /// Gobal Color/Tint of the <see cref="IEntity"/>
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        Single Alpha { get; set; }

        /// <summary>
        /// Current Role that descripes the <see cref="IEntity"/>s Behaviour
        /// </summary>
        Role CurrentRole { get; }


        // Methods ######################################################################
        /// <summary>
        /// Updates the <see cref="IEntity"/> using its applied Role.
        /// Can be overriden by derived classes.
        /// </summary>
        /// <param name="gameTime">Current gametime</param>
        void Update(Single deltaT);

        /// <summary>
        /// Renders the <see cref="IEntity"/> into the scene
        /// </summary>
        void Draw();


        // Roles ########################################################################
        /// <summary>
        /// Assigns a new Role to the <see cref="IEntity"/> without removing the current one.
        /// </summary>
        /// <param name="role">The Role to assign</param>
        /// <param name="supressInitialization">Supress initialization call on assigned role</param>
        void AssignRole(Role role, Boolean supressInitialization = false);

        /// <summary>
        /// Assigns a new Role to the <see cref="IEntity"/> after removing the current one.
        /// </summary>
        /// <param name="role">The Role to assign</param>
        /// <param name="supressInitialization">Supress initialization call on assigned role</param>
        /// <returns>The removed role if there was one - otherwhise null</returns>
        Role ReplaceRole(Role role, Boolean supressInitialization = false);

        /// <summary>
        /// Removes the currently active Role from this <see cref="IEntity"/>
        /// </summary>
        /// <returns>The removed role if there was one - otherwhise null</returns>
        Role RemoveRole();
    }
}