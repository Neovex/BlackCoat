using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class ParticleAnimationInfo
    {
        public virtual float TTL { get; set; }
        public virtual Vector2f Offset { get; set; }
        public virtual Color Color { get; set; }
        public virtual Vector2f Velocity { get; set; }
        public virtual Vector2f Acceleration { get; set; }
        public virtual float Alpha { get; set; }
        public virtual float AlphaFade { get; set; }
        public virtual bool UseAlphaAsTTL { get; set; }

        public ParticleAnimationInfo()
        {
            TTL = 1;
            Color = Color.White;
            Alpha = 1;
        }
    }
}