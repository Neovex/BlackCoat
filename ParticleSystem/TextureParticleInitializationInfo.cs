using System;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class TextureParticleInitializationInfo : PixelParticleInitializationInfo
    {
        public Texture Texture { get; }

        public virtual Vector2f Origin { get; set; }
        public virtual Vector2f Scale { get; set; }
        public virtual Vector2f ScaleVelocity { get; set; }
        public virtual float Rotation { get; set; }
        public virtual float RotationVelocity { get; set; }

        public TextureParticleInitializationInfo(Texture texture) : base()
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Scale = new Vector2f(1, 1);
        }
    }
}