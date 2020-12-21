using SFML.System;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Common interface for all particle implementations which accept particle spawn information.
    /// </summary>
    /// <typeparam name="T">Type of the initialization package. Usually inherited from <see cref="ParticleSpawnInfo"/>.</typeparam>
    public interface IInitializableByInfo<T>
    {
        // Methods #########################################################################
        /// <summary>
        /// Initializes the particle with the provided animation parameters.
        /// </summary>
        /// <param name="position">The position of the parent emitter.</param>
        /// <param name="info">The initialization package information.</param>
        void Initialize(Vector2f position, T info);
    }
}