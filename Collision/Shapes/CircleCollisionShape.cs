using SFML.System;

namespace BlackCoat.Collision.Shapes
{
    /// <summary>
    /// Represents a simple Circle for Collision Detection.
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.CollisionShape"/>
    /// <seealso cref="BlackCoat.Collision.ICircle"/>
    public class CircleCollisionShape : CollisionShape, ICircle
    {
        /// <summary>
        /// Position of the <see cref="CircleCollisionShape"/>.
        /// </summary>
        public Vector2f Position { get; set; }

        /// <summary>
        /// Radius of the <see cref="CircleCollisionShape"/>
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Circle;


        /// <summary>
        /// Initializes a new instance of the <see cref="CircleCollisionShape"/> class.
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        /// <param name="position">The <see cref="CircleCollisionShape"/>'s position.</param>
        /// <param name="radius">The <see cref="CircleCollisionShape"/>'s radius.</param>
        public CircleCollisionShape(CollisionSystem collisionSystem, Vector2f position, float radius) : base(collisionSystem)
        {
            Position = position;
            Radius = radius;
        }


        /// <summary>
        /// Determines if this <see cref="CircleCollisionShape"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="CircleCollisionShape"/></returns>
        public override bool Collide(Vector2f point)
        {
            return _CollisionSystem.CheckCollision(point, this);
        }

        /// <summary>
        /// Determines if this <see cref="CircleCollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objetcs overlap or touch</returns>
        public override bool Collide(ICollisionShape other)
        {
            return _CollisionSystem.CheckCollision(this, other);
        }
    }
}