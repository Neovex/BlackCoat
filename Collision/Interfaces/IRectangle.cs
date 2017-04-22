using SFML.System;

namespace BlackCoat.Collision
{
    public interface IRectangle : ICollisionShape
    {
        /// <summary>
        /// Location of the <see cref="IRectangle"/> within its parent container
        /// </summary>
        Vector2f Position { get; }

        /// <summary>
        /// Size of the <see cref="IRectangle"/>
        /// </summary>
        Vector2f Size { get; }
    }
}