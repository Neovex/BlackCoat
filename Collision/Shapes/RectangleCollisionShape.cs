using System;
using SFML.System;

namespace BlackCoat.Collision
{
    /// <summary>
    /// Represents a simple Rectangle for Collision Detection.
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.CollisionShape" />
    /// <seealso cref="BlackCoat.Collision.IRectangle" />
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
        public override Geometry CollisionGeometry { get { return Geometry.Rectangle; } }


        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleCollisionShape"/> class.
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        /// <param name="position">The Rectangle position.</param>
        /// <param name="size">The Rectangle size.</param>
        public RectangleCollisionShape(CollisionSystem collisionSystem, Vector2f position, Vector2f size) : base(collisionSystem)
        {
            Position = position;
            Size = size;
        }

        /// <summary>
        /// Determines if this <see cref="RectangleCollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objetcs overlap or touch</returns>
        public override bool Collide(ICollisionShape other)
        {
            switch (other.CollisionGeometry)
            {
                case Geometry.Line: return _CollisionSystem.CheckCollision(this, other as ILine);
                case Geometry.Circle: return _CollisionSystem.CheckCollision(other as ICircle, this);
                case Geometry.Rectangle: return _CollisionSystem.CheckCollision(this, other as IRectangle);
                case Geometry.Polygon: return _CollisionSystem.CheckCollision(this, other as IPoly);
            }

            Log.Error("Invalid collision shape", other, other?.CollisionGeometry);
            throw new Exception("Invalid collision shape");
        }
    }
}