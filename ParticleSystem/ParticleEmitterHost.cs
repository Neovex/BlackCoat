using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Graphics;
using BlackCoat.Entities;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// This class represents the bridge between the Particle System and the State/Entity scene graph.
    /// </summary>
    /// <seealso cref="BlackCoat.Entities.EntityBase" />
    public sealed class ParticleEmitterHost : EntityBase
    {
        private readonly List<EmitterBase> _Emitters;
        private readonly SortedList<int, List<ParticleVertexRenderer>> _DepthLayers;

        /// <summary>
        /// The Color of the <see cref="ParticleEmitterHost"/> has no effect on emitters.
        /// Use the appropriate color property of the corresponding <see cref="PixelParticleInitializationInfo"/> of each emitter.
        /// </summary>
        public override Color Color { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleEmitterHost"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        public ParticleEmitterHost(Core core) : base(core)
        {
            _Emitters = new List<EmitterBase>();
            _DepthLayers = new SortedList<int, List<ParticleVertexRenderer>>();
        }

        internal IEnumerable<EmitterBase> Emitters => _Emitters;
        internal IEnumerable<ParticleVertexRenderer> DepthLayers => _DepthLayers.Values.SelectMany(l => l);

        /// <summary>
        /// Adds an emitter to the host.
        /// </summary>
        /// <param name="emitter">The emitter to add.</param>
        /// <exception cref="ArgumentNullException">emitter</exception>
        public void AddEmitter(EmitterBase emitter)
        {
            if (emitter == null) throw new ArgumentNullException(nameof(emitter));

            if (_Emitters.Contains(emitter)) return;
            _Emitters.Add(emitter);

            if (emitter is EmitterComposition composite)
            {
                composite.Host = this;
            }
            else
            {
                if (!_DepthLayers.TryGetValue(emitter.Depth, out List<ParticleVertexRenderer> layer))
                {
                    layer = new List<ParticleVertexRenderer>();
                    _DepthLayers.Add(emitter.Depth, layer);
                }

                var vertexRenderer = layer.FirstOrDefault(vr => vr.IsCompatibleWith(emitter));
                if (vertexRenderer == null)
                {
                    vertexRenderer = new ParticleVertexRenderer(_Core, emitter.PrimitiveType, emitter.BlendMode, emitter.Texture);
                    layer.Add(vertexRenderer);
                }
                vertexRenderer.AssociatedEmitters++;
                emitter.Initialize(vertexRenderer);
            }
        }

        /// <summary>
        /// Removes the specified emitter from the host.
        /// </summary>
        /// <param name="emitter">The emitter to be removed.</param>
        /// <exception cref="ArgumentNullException">emitter</exception>
        public void Remove(EmitterBase emitter)
        {
            if(emitter == null) throw new ArgumentNullException(nameof(emitter));

            if (!_Emitters.Contains(emitter)) return;
            _Emitters.Remove(emitter);
            emitter.Cleanup();

            if (emitter is EmitterComposition composite)
            {
                composite.Host = null;
            }
            else
            {
                var layer = _DepthLayers[emitter.Depth];
                var vertexRenderer = layer.First(vr => vr.IsCompatibleWith(emitter));
                vertexRenderer.AssociatedEmitters--;
                if (vertexRenderer.AssociatedEmitters == 0) layer.Remove(vertexRenderer);
                if (layer.Count == 0) _DepthLayers.Remove(emitter.Depth);
            }
        }

        /// <summary>
        /// Updates the <see cref="IEntity" />.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public override void Update(float deltaT)
        {
            foreach (var emitter in _Emitters)
            {
                emitter.UpdateInternal(deltaT);
            }
        }

        /// <summary>
        /// Renders the <see cref="IEntity" /> into the scene.
        /// </summary>
        /// <param name="target">Render device</param>
        /// <param name="states">Additional render information</param>
        public override void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var layer in _DepthLayers.Values)
            {
                foreach (var vertexRenderer in layer)
                {
                    vertexRenderer.Draw(target, states);
                }
            }
        }
    }
}