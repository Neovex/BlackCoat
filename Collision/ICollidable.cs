namespace BlackCoat.Collision
{
    public interface ICollidable
    {
        ICollisionShape CollisionShape { get; }
    }
}