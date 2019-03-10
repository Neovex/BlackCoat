using System;
using SFML.System;
using BlackCoat.Entities;

namespace BlackCoat.Collision.Shapes
{
    /// <summary>
    /// This class provides a Rectangular Collision Shape based on a associated <see cref="TextItem"/> instance.
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.CollisionShape" />
    /// <seealso cref="BlackCoat.Collision.IRectangle" />
    public class BasicTextCollisionShape : CollisionShape, IRectangle
    {
        private TextItem _Text;

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Rectangle;

        /// <summary>
        /// Gets the position of the associated <see cref="TextItem"/>.
        /// </summary>
        public Vector2f Position => _Text.GlobalPosition - (_Text.Origin + _Text.GetLocalBounds().Position());

        /// <summary>
        /// Gets the size of the associated <see cref="TextItem"/>.
        /// </summary>
        public Vector2f Size => _Text.GetLocalBounds().Size();


        /// <summary>
        /// Initializes a new instance of the <see cref="BasicTextCollisionShape"/> class.
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        /// <param name="text">The <see cref="TextItem"/> that will be associated with this <see cref="BasicTextCollisionShape"/>.</param>
        public BasicTextCollisionShape(CollisionSystem collisionSystem, TextItem text) : base(collisionSystem)
        {
            _Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Determines if this <see cref="BasicTextCollisionShape"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="BasicTextCollisionShape"/></returns>
        override public bool CollidesWith(Vector2f point)
        {
            return _CollisionSystem.CheckCollision(point, this);
        }

        /// <summary>
        /// Determines if this <see cref="BasicTextCollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        override public bool CollidesWith(ICollisionShape other)
        {
            return _CollisionSystem.CheckCollision(this, other);
        }
    }
}