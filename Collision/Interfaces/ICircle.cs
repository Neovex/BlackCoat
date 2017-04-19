using SFML.System;

namespace BlackCoat.Collision
{
    public interface ICircle : ICollisionShape
    {
        Vector2f Position { get; set; }
        float Radius { get; set; }
    }
}