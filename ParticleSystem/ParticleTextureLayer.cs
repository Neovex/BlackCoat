using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    internal class ParticleTextureLayer : VertexRenderer
    {
        public Texture Texture { get; }
        public bool IsEmpty => Verticies.Length == 0;

        public BlendMode BlendMode { get; set; }


        public ParticleTextureLayer(Texture texture, BlendMode blendMode) : base(4)
        {
            Texture = texture;
            BlendMode = blendMode;
        }


        public void Add(Emitter emitter)
        {
            emitter.VertexRenderer = this;
        }

        public void Remove(Emitter emitter)
        {
            emitter.VertexRenderer = null;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Texture = Texture;
            states.BlendMode = BlendMode;
            target.Draw(Verticies, PrimitiveType.Quads, states);
        }
    }
}
