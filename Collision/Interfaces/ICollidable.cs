namespace BlackCoat.Collision
{
    public interface ICollidable
    {
        /// <summary>
        /// Gets the geometric collision information associated with this object
        /// </summary>
        ICollisionShape CollisionShape { get; }
    }
}