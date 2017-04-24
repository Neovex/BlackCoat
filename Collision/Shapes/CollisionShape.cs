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
        /// Determines if this  <see cref="CollisionShape"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the object</returns>
        public abstract bool Collide(Vector2f point);

        /// <summary>
        /// Determines if this <see cref="CollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objetcs overlap or touch</returns>
        public abstract bool Collide(ICollisionShape other);
    }
}