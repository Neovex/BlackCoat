using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public interface IInitializableByInfo<T>
    {
        void Initialize(Vector2f position, T info);
    }
}