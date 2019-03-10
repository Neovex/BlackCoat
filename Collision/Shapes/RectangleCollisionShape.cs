using SFML.System;

namespace BlackCoat.Collision.Shapes
{
    /// <summary>
    /// Represents a simple Rectangle for Collision Detection.
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.CollisionShape"/>
    /// <seealso cref="BlackCoat.Collision.IRectangle"/>
    public class RectangleCollisionShape : CollisionShape, IRectangle
    {
        /// <summary>
        /// Rectangle position.
        /// </summary>
        public Vector2f Position { get; set; }

        /// <summary>
        /// Rectangle size.
        /// </summary>
        public Vector2f Size { get; set; }

        /// <summary>
        /// Determines the geometric primitive used for collision detection. In this case a Rectangle.
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Rectangle;


        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleCollisionShape"/> class.
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        /// <param name="position">The <see cref="RectangleCollisionShape"/> position.</param>
        /// <param name="size">The <see cref="RectangleCollisionShape"/> size.</param>
        public RectangleCollisionShape(CollisionSystem collisionSystem, Vector2f position, Vector2f size) : base(collisionSystem)
        {
            Position = position;
            Size = size;
        }


        /// <summary>
        /// Determines if this <see cref="RectangleCollisionShape"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="RectangleCollisionShape"/></returns>
        override public bool CollidesWith(Vector2f point)
        {
            return _CollisionSystem.CheckCollision(point, this);
        }

        /// <summary>
        /// Determines if this <see cref="RectangleCollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objetcs overlap or touch</returns>
        override public bool CollidesWith(ICollisionShape other)
        {
            return _CollisionSystem.CheckCollision(this, other);
        }
    }
}