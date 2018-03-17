using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    internal class ParticleDepthLayer : VertexRenderer
    {
        private List<ParticleTextureLayer> _TextureLayers;

        public int Depth { get; }

        public bool IsEmpty => _TextureLayers.Count == 0 && Verticies == null;


        public ParticleDepthLayer(int depth) : base(1)
        {
            Depth = depth;
            _TextureLayers = new List<ParticleTextureLayer>();
        }


        internal void Add(PixelEmitter emitter)
        {
            if (emitter is Emitter)
            {
                var e = emitter as Emitter;
                var layer = _TextureLayers.FirstOrDefault(l => l.Texture == e.Texture);

                if (layer == null)
                {
                    layer = new ParticleTextureLayer(e.Texture, e.BlendMode);
                    _TextureLayers.Add(layer);
                }

                layer.Add(e);
            }
            else
            {
                emitter.VertexRenderer = this;
            }
        }

        internal void Remove(PixelEmitter emitter)
        {
            if (emitter is Emitter)
            {
                var e = emitter as Emitter;
                var layer = _TextureLayers.First(l => l.Texture == e.Texture);
                layer.Remove(e);
                if (layer.IsEmpty) _TextureLayers.Remove(layer);
            }
            else
            {
                emitter.VertexRenderer = null;
            }
        }

        override public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var texLayer in _TextureLayers)
            {
                texLayer.Draw(target, states);
            }

            if (Verticies.Length != 0)
            {
                target.Draw(Verticies, PrimitiveType.Points, states);
            }
        }
    }
}