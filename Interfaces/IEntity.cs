using System;
using SFML.Graphics;
using BlackCoat.Entities;
using SFML.System;

namespace BlackCoat
{
    /// <summary>
    /// Common interface of all <see cref="BlackCoat"/> Entity Types
    /// </summary>
    public interface IEntity:Drawable, IDisposable
    {
        // Properties ######################################################################
        /// <summary>
        /// Name of the <see cref="IEntity"/>
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// Gets or sets the position of this <see cref="IEntity"/>
        /// </summary>
        Vector2f Position { get; set; }

        /// <summary>
        /// Gets the position of this <see cref="IEntity"/> independent from scene graph and view.
        /// </summary>
        Vector2f GlobalPosition { get; }

        /// <summary>
        /// Rotation of the <see cref="IEntity"/>
        /// </summary>
        float Rotation { get; set; }

        /// <summary>
        /// Scale of the <see cref="IEntity"/>
        /// </summary>
        Vector2f Scale { get; set; }

        /// <summary>
        /// The origin of an object defines the center point for transformations
        /// </summary>
        Vector2f Origin { get; set; }

        /// <summary>
        /// Parent Container of the <see cref="IEntity"/>
        /// </summary>
        Container Parent { get; set; }

        /// <summary>
        /// Determines the Visibility of the <see cref="IEntity"/>
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
        /// Shader for Rendering
        /// </summary>
        Shader Shader { get; set; }

        /// <summary>
        /// Blending method used for Rendering
        /// </summary>
        BlendMode BlendMode { get; set; }

        /// <summary>
        /// Global Color/Tint of the <see cref="IEntity"/>
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        Single Alpha { get; set; }

        /// <summary>
        /// Target device for rendering
        /// </summary>
        RenderTarget RenderTarget { get; set; }

        /// <summary>
        /// Determines whether this <see cref="IEntity" /> is destroyed.
        /// </summary>
        Boolean Disposed { get; }


        // Methods ######################################################################
        /// <summary>
        /// Updates the <see cref="IEntity"/> using its applied Role.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        void Update(Single deltaT);

        /// <summary>
        /// Renders the <see cref="IEntity"/> into the scene
        /// </summary>
        void Draw();
    }
}