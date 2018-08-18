using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class TextureParticleAnimationInfo : ParticleAnimationInfo
    {
        public virtual Vector2f Origin { get; set; }
        public virtual Vector2f Scale { get; set; }
        public virtual Vector2f ScaleVelocity { get; set; }
        public virtual float Rotation { get; set; }
        public virtual float RotationVelocity { get; set; }

        public TextureParticleAnimationInfo() : base()
        {
            Scale = new Vector2f(1, 1);
        }
    }
}