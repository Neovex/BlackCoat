using SFML.Graphics;
using BlackCoat.Collision;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a Circle Primitive
    /// </summary>
    public class Circle : ShapeEntity<CircleShape>, ICircle
    {
        // Properties ######################################################################
        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        public float Radius { get => Target.Radius; set => Target.Radius = value; }

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Circle;


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new <see cref="Circle" /> instance
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="radius">The radius</param>
        /// <param name="fillColor">Color of the shapes area.</param>
        /// <param name="outlineColor">Color of the shapes outline.</param>
        public Circle(Core core, float radius, Color? fillColor = null, Color? outlineColor = null) :
                      base(core, new CircleShape(radius), fillColor, outlineColor)
        {
        }
    }
}