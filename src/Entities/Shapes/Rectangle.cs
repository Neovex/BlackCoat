using SFML.System;
using SFML.Graphics;
using BlackCoat.Collision;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a Rectangle Primitive
    /// </summary>
    public class Rectangle : ShapeEntity<RectangleShape>, IRectangle
    {
        // Properties ######################################################################
        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        public Vector2f Size { get => Target.Size; set => Target.Size = value; }

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Rectangle;


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new <see cref="Rectangle" /> instance
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="size"><see cref="Rectangle" /> dimensions</param>
        /// <param name="fillColor">Color of the shapes area.</param>
        /// <param name="outlineColor">Color of the shapes outline.</param>
        public Rectangle(Core core, Vector2f size, Color? fillColor = null, Color? outlineColor = null) : base(core, new RectangleShape(size), fillColor, outlineColor)
        {
        }
    }
}