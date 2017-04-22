using SFML.System;

namespace BlackCoat.Collision
{
    public interface ILine : ICollisionShape
    {
        /// <summary>
        /// Startposition of the <see cref="ILine"/>. Read only.
        /// </summary>
        Vector2f Start { get; }

        /// <summary>
        /// Endposition of the <see cref="ILine"/>. Read only.
        /// </summary>
        Vector2f End { get; }
    }
}