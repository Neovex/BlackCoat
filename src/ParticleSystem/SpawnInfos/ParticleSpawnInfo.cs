using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class ParticleSpawnInfo
    {
        // Properties ######################################################################
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

        /// <summary>
        /// Gets or sets the offset between the particle and its emitter.
        /// </summary>
        public virtual Vector2f Offset { get; set; }

        /// <summary>
        /// Gets or sets the particle velocity.
        /// </summary>
        public virtual Vector2f Velocity { get; set; }

        /// <summary>
        /// Gets or sets the acceleration which will modify the velocity over time.
        /// </summary>
        public virtual Vector2f Acceleration { get; set; }

        /// <summary>
        /// Gets or sets the particle tint color.
        /// </summary>
        public virtual Color Color { get; set; }

        /// <summary>
        /// Gets or sets the particles alpha visibility.
        /// </summary>
        public virtual float Alpha { get; set; }

        /// <summary>
        /// Gets or sets the alpha fade which will change the alpha value over time
        /// </summary>
        public virtual float AlphaFade { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the alpha value will be utilized as TTL.
        /// When this is set to <code>true</code> and alpha becomes 0 or less the particle will be recycled.
        /// </summary>
        public virtual bool UseAlphaAsTTL { get; set; }


        // CTOR ############################################################################        
        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleSpawnInfo"/> class.
        /// </summary>
        public ParticleSpawnInfo()
        {
            TTL = 1;
            ParticlesPerSpawn = 1;

            Color = Color.White;
            Alpha = 1;
        }
    }
}