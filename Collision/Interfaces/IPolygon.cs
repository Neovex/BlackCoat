using System.Collections.Generic;
using SFML.System;

namespace BlackCoat.Collision
{
    public interface IPolygon : ICollisionShape
    {
        /// <summary>
        /// Location of the <see cref="IPolygon"/> within its parent container
        /// </summary>
        Vector2f Position { get; }

        /// <summary>
        /// Vectors this <see cref="IPolygon"/> is composed of. Read Only.
        /// </summary>
        IReadOnlyList<Vector2f> Points { get; }
    }
}