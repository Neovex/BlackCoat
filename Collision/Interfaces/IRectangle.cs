using SFML.System;

namespace BlackCoat.Collision
{
    public interface IRectangle : ICollisionShape
    {
        Vector2f Position { get; }
        Vector2f Size { get; }
    }
}