using System;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class TexturedSpawnInfo : ParticleSpawnInfo
    {
        // Properties ######################################################################        
        /// <summary>
        /// Read only. Gets the texture used on each particle.
        /// </summary>
        public Texture Texture { get; }

        /// <summary>
        /// Gets or sets each particles origin.
        /// </summary>
        public virtual Vector2f Origin { get; set; }

        /// <summary>
        /// Gets or sets each particles scale.
        /// </summary>
        public virtual Vector2f Scale { get; set; }

        /// <summary>
        /// Gets or sets the scale velocity of each particle which will modify its scale over time.
        /// </summary>
        public virtual Vector2f ScaleVelocity { get; set; }

        /// <summary>
        /// Gets or sets the particles rotation.
        /// </summary>
        public virtual float Rotation { get; set; }

        /// <summary>
        /// Gets or sets the rotation velocity of each particle which will modify its rotation over time.
        /// </summary>
        public virtual float RotationVelocity { get; set; }


        // CTOR ############################################################################        
        /// <summary>
        /// Initializes a new instance of the <see cref="TexturedSpawnInfo"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <exception cref="ArgumentNullException">texture</exception>
        public TexturedSpawnInfo(Texture texture)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Scale = new Vector2f(1, 1);
        }
    }
}