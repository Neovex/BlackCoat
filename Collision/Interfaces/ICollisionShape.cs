namespace BlackCoat.Collision
{
    public interface ICollisionShape
    {
        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        Geometry CollisionGeometry { get; }

        /// <summary>
        /// Determines if this object is colliding with the defined other
        /// </summary>
        /// <param name="other">The other object</param>
        /// <returns>True when the objetcs overlap or touch</returns>
        bool Collide(ICollisionShape other);
    }
}