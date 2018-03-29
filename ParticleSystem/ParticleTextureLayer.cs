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


        public ParticleTextureLayer(Core core, Texture texture, BlendMode blendMode) : base(core, 4)
        {
            Texture = texture;
            BlendMode = blendMode;
        }


        public void Add(TextureEmitter emitter)
        {
            emitter.Initialize(this);
        }

        public void Remove(TextureEmitter emitter)
        {
            emitter.Cleanup();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Texture = Texture;
            states.BlendMode = BlendMode;
            target.Draw(Verticies, PrimitiveType.Quads, states);
        }
    }
}
