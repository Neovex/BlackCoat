using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Defines the first render hierarchy inside a <see cref="ParticleEmitterHost"/>.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.VertexRenderer" />
    internal class ParticleDepthLayer : VertexRenderer
    {
        private List<ParticleTextureLayer> _TextureLayers;

        /// <summary>
        /// Gets the depth of this layer.
        /// </summary>
        public int Depth { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty => _TextureLayers.Count == 0 && Verticies == null;

        internal IEnumerable<ParticleTextureLayer> TextureLayers => _TextureLayers;


        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleDepthLayer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="depth">The depth of the instance.</param>
        public ParticleDepthLayer(Core core, int depth) : base(core, 1)
        {
            Depth = depth;
            _TextureLayers = new List<ParticleTextureLayer>();
        }


        /// <summary>
        /// Adds the specified emitter.
        /// </summary>
        /// <param name="emitter">The emitter to be added.</param>
        internal void Add(BaseEmitter emitter)
        {
            if (emitter is TextureEmitter texEmitter)
            {
                var layer = _TextureLayers.FirstOrDefault(l => l.Texture == texEmitter.Texture);
                if (layer == null)
                {
                    layer = new ParticleTextureLayer(_Core, texEmitter.Texture, texEmitter.BlendMode);
                    _TextureLayers.Add(layer);
                }
                layer.Add(texEmitter);
            }
            else
            {
                emitter.Initialize(this);
            }
        }

        /// <summary>
        /// Removes the specified emitter.
        /// </summary>
        /// <param name="emitter">The emitter to be removed.</param>
        internal void Remove(BaseEmitter emitter)
        {
            if (emitter is TextureEmitter texEmitter)
            {
                var layer = _TextureLayers.First(l => l.Texture == texEmitter.Texture);
                layer.Remove(texEmitter);
                if (layer.IsEmpty) _TextureLayers.Remove(layer);
            }
            else
            {
                emitter.Cleanup();
            }
        }

        /// <summary>
        /// Draws all active vertices on to the defined render target.
        /// </summary>
        /// <param name="target">The render target.</param>
        /// <param name="states">Additional render information.</param>
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