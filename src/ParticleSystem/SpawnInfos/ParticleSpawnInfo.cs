using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class ParticleSpawnInfo
    {
        /// <summary>
        /// Lifetime of the particles.
        /// </summary>
        public virtual float TTL { get; set; }
        /// <summary>
        /// The amount of particles that should be emitted during spawn phase.
        /// </summary>
        public virtual uint ParticlesPerSpawn { get; set; }
        /// <summary>
        /// This determines if the emitter needs to be re-triggered or runs continuously.
        /// </summary>
        public virtual bool Loop { get; set; }
        /// <summary>
        /// Only relevant when loop = true. The spawn rate defines the time between each spawn phases.
        /// </summary>
        public virtual float SpawnRate { get; set; }

        public virtual Vector2f Offset { get; set; }
        public virtual Vector2f Velocity { get; set; }
        public virtual Vector2f Acceleration { get; set; }
        public virtual Color Color { get; set; }
        public virtual float Alpha { get; set; }
        public virtual float AlphaFade { get; set; }
        public virtual bool UseAlphaAsTTL { get; set; }

        public ParticleSpawnInfo()
        {
            TTL = 1;
            ParticlesPerSpawn = 1;

            Color = Color.White;
            Alpha = 1;
        }
    }
}