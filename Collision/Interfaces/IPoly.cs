using System.Collections.Generic;
using SFML.System;

namespace BlackCoat.Collision
{
    public interface IPoly : ICollisionShape
    {
        Vector2f Position { get; set; }
        IReadOnlyList<Vector2f> Points { get; }
    }
}