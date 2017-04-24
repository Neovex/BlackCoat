using SFML.System;

namespace BlackCoat.Collision
{
    public interface ICircle : ICollisionShape
    {
        /// <summary>
        /// Location of the <see cref="ICircle"/> within its parent container
        /// </summary>
        Vector2f Position { get; }

        /// <summary>
        /// Radius of the <see cref="ICircle"/>
        /// </summary>
        float Radius { get; }
    }
}