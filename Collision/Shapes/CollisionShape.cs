using System;
using SFML.System;

namespace BlackCoat.Collision
{
    /// <summary>
    /// Abstract base class of all Collision Shapes
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.ICollisionShape" />
    public abstract class CollisionShape : ICollisionShape
    {
        protected CollisionSystem _CollisionSystem;

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public abstract Geometry CollisionGeometry { get; }

        /// <summary>
        /// Initializes the base class <see cref="CollisionShape"/>
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        public CollisionShape(CollisionSystem collisionSystem)
        {
            if (collisionSystem == null) throw new ArgumentNullException(nameof(collisionSystem));
            _CollisionSystem = collisionSystem;
        }

        /// <summary>
        /// Determines if this object is contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the object</returns>
        public abstract bool Collide(Vector2f point);

        /// <summary>
        /// Determines if this object is colliding with the defined other
        /// </summary>
        /// <param name="other">The other object</param>
        /// <returns>True when the objetcs overlap or touch</returns>
        public abstract bool Collide(ICollisionShape other);
    }
}