using SFML.System;

namespace BlackCoat.Collision
{
    public interface ILine : ICollisionShape
    {
        Vector2f Start { get; set; }
        Vector2f End { get; }
    }
}