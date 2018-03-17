using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;

using BlackCoat;
using BlackCoat.Entities;

namespace BlackCoat.ParticleSystem
{
    class ParticleGroup : BaseEntity
    {
        private List<Vertex> _Vertices;
        private Vertex[] _VerticesForRendering;


        public override Color Color { get; set; }
        public Texture Texture
        {
            get { return RenderState.Texture; }
            set
            {
                var state = RenderState;
                state.Texture = value;
                RenderState = state;
            }
        }

        public List<BlackCoat.ParticleSystem.Particle> Particles
        {
            get => default(Particle);
            set
            {
            }
        }

        public ParticleGroup(Core core, int initialSize) : base(core)
        {
            _Vertices = new List<Vertex>(initialSize);
        }


        public void Add(int index, Vector2f pos, Vector2i tex)
        {
            var c = Color;
            if (tex.X < 0) c.A = 0;

            var v = new Vertex();
            v.Position.X = pos.X;
            v.Position.Y = pos.Y;
            v.TexCoords.X = tex.X;
            v.TexCoords.Y = tex.Y;
            v.Color = c;
            _Vertices.Add(v);


            v.Position.X = pos.X + _TileSize.X;
            v.Position.Y = pos.Y;
            v.TexCoords.X = tex.X + _TileSize.X;
            v.TexCoords.Y = tex.Y;
            v.Color = c;
            _Vertices.Add(v);


            v.Position.X = pos.X + _TileSize.X;
            v.Position.Y = pos.Y + _TileSize.Y;
            v.TexCoords.X = tex.X + _TileSize.X;
            v.TexCoords.Y = tex.Y + _TileSize.Y;
            v.Color = c;
            _Vertices.Add(v);


            v.Position.X = pos.X;
            v.Position.Y = pos.Y + _TileSize.Y;
            v.TexCoords.X = tex.X;
            v.TexCoords.Y = tex.Y + _TileSize.Y;
            v.Color = c;
            _Vertices.Add(v);
        }

        public override void Draw()
        {
            _Core.Draw(this);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Texture = Texture;
            target.Draw(_Vertices, PrimitiveType.Quads, states);
        }

        public void Kill(int index)
        {
            throw new System.NotImplementedException();
        }

        public unsafe override void Update(float deltaT)
        {
            fixed (Vertex* fptr = _VerticesForRendering)
            {
                var ptr = fptr;
                for (int i = 0; i < Particles.Count; i++)
                {
                    Particles[i].Update(deltaT, ptr);
                    ptr += 4;
                }
            }
        }
    }
}