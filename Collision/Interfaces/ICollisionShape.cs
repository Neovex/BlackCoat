using SFML.System;

namespace BlackCoat.Collision
{
    public interface ICollisionShape
    {
        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        Geometry CollisionGeometry { get; }
        
        /// <summary>
        /// Determines if this object is contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the object</returns>
        bool Collide(Vector2f point);

        /// <summary>
        /// Determines if this object is colliding with the defined other
        /// </summary>
        /// <param name="other">The other object</param>
        /// <returns>True when the objetcs overlap or touch</returns>
        bool Collide(ICollisionShape other);
    }
}