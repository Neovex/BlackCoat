using System;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class BasicPixelParticle : PixelParticle
    {
        protected Vector2f _Velocity;
        protected Vector2f _Acceleration;

        public BasicPixelParticle(Core core) : base(core)
        {
        }


        public virtual void Initialize(Vector2f position, Color color, Vector2f velocity, Vector2f acceleration)
        {
            _Position = position;
            _Color = color;

            _Velocity = velocity;
            _Acceleration = acceleration;
        }

        override protected unsafe bool UpdateInternal(float deltaT, Vertex* vPtr)
        {
            _Velocity += _Acceleration * deltaT;
            _Position += _Velocity * deltaT;
            return base.UpdateInternal(deltaT, vPtr);
        }
    }
}