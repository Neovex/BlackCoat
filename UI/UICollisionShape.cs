using System;
using SFML.System;
using BlackCoat.Collision;
using BlackCoat.Collision.Shapes;

namespace BlackCoat.UI
{
    /// <summary>
    /// This class provides a Rectangular Collision Shape based on a associated <see cref="UIComponent"/> instance.
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.CollisionShape" />
    /// <seealso cref="BlackCoat.Collision.IRectangle" />
    public class UICollisionShape : CollisionShape, IRectangle
    {
        private UIComponent _Component;

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Rectangle;

        /// <summary>
        /// Gets the position of the associated <see cref="UIComponent"/>.
        /// </summary>
        public Vector2f Position => _Component.GlobalPosition;

        /// <summary>
        /// Gets the size of the associated <see cref="UIComponent"/>.
        /// </summary>
        public Vector2f Size => _Component.InnerSize;


        /// <summary>
        /// Initializes a new instance of the <see cref="UICollisionShape"/> class.
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        /// <param name="component">The <see cref="UIComponent"/> that will be associated with this <see cref="UICollisionShape"/>.</param>
        public UICollisionShape(CollisionSystem collisionSystem, UIComponent component) : base(collisionSystem)
        {
            _Component = component ?? throw new ArgumentNullException(nameof(component));
        }

        /// <summary>
        /// Determines if this <see cref="UICollisionShape"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="UICollisionShape"/></returns>
        override public bool Collide(Vector2f point) => _CollisionSystem.CheckCollision(point, this);

        /// <summary>
        /// Determines if this <see cref="UICollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        override public bool Collide(ICollisionShape other) => _CollisionSystem.CheckCollision(this, other);
    }
}