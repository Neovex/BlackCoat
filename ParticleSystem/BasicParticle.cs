using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class BasicParticle
    {
        internal static int _PARTICLES = 0;

        private int _Index;
        protected VertexRenderer _VertexRenderer;

        public float TTL { get; set; }
        public Vector2f Position { get; set; }
        public Color Color { get; set; }

        public Vector2f Velocity { get; set; }
        public Vector2f Acceleration { get; set; }

        public BasicParticle()
        {
        }

        public virtual void Initialize(VertexRenderer renderer, float ttl, Vector2f position, Vector2f velocity, Vector2f acceleration, Color color)
        {
            _VertexRenderer = renderer;
            _Index = _VertexRenderer.Reserve();
            TTL = ttl;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            Color = color;

            _PARTICLES++;
        }


        internal void Free()
        {
            _VertexRenderer.Free(_Index);

            _PARTICLES--;
        }

        /*
        public virtual bool Update(float deltaT)
        {
            TTL -= deltaT;
            Velocity += Acceleration * deltaT;
            Position += Velocity * deltaT;
            _VertexRenderer.Verticies[_Index] = new Vertex(Position, Color);
            return TTL < 0;
        }
        */
        public virtual unsafe void Update(float deltaT)
        {
            TTL -= deltaT;
            Velocity += Acceleration * deltaT;
            Position += Velocity * deltaT;

            fixed (Vertex* fptr = _VertexRenderer.Verticies)
            {
                var ptr = fptr + _Index;
                ptr->Position = Position;
                ptr->Color = Color;
            }

            //return TTL < 0;
        }
    }
}