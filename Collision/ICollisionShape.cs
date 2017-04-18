namespace BlackCoat.Collision
{
    public interface ICollisionShape
    {
        Geometry CollisionGeometry { get; }
        bool Collide(ICollisionShape other);
    }
}