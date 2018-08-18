using System;
using System.Collections.Generic;
using SFML.Graphics;
using BlackCoat.Entities;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// This class represents the bridge between the Particle System and the State/Entity scene graph.
    /// </summary>
    /// <seealso cref="BlackCoat.Entities.BaseEntity" />
    public class ParticleEmitterHost : BaseEntity
    {
        private List<BaseEmitter> _Emitters;
        private SortedList<int, ParticleDepthLayer> _DepthLayers;

        /// <summary>
        /// The Color of the <see cref="ParticleEmitterHost"/> has no effect on emitters.
        /// Use the appropriate color property of the corresponding <see cref="ParticleAnimationInfo"/> of each emitter.
        /// </summary>
        public override Color Color { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleEmitterHost"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        public ParticleEmitterHost(Core core) : base(core)
        {
            _Emitters = new List<BaseEmitter>();
            _DepthLayers = new SortedList<int, ParticleDepthLayer>();
        }

        internal IEnumerable<ParticleDepthLayer> DepthLayers => _DepthLayers.Values;
        internal IEnumerable<BaseEmitter> Emitters => _Emitters;

        /// <summary>
        /// Adds an emitter to the host.
        /// </summary>
        /// <param name="emitter">The emitter to add.</param>
        /// <exception cref="ArgumentNullException">emitter</exception>
        public void AddEmitter(BaseEmitter emitter)
        {
            if (emitter == null) throw new ArgumentNullException(nameof(emitter));

            _Emitters.Add(emitter);
            if (emitter is CompositeEmitter composite)
            {
                composite.Host = this;
            }
            else
            {
                if (!_DepthLayers.ContainsKey(emitter.Depth))
                {
                    _DepthLayers.Add(emitter.Depth, new ParticleDepthLayer(_Core, emitter.Depth));
                }
                _DepthLayers[emitter.Depth].Add(emitter);
            }
        }

        /// <summary>
        /// Removes the specified emitter from the host.
        /// </summary>
        /// <param name="emitter">The emitter to be removed.</param>
        /// <exception cref="ArgumentNullException">emitter</exception>
        public void Remove(BaseEmitter emitter)
        {
            if(emitter == null) throw new ArgumentNullException(nameof(emitter));
            _Emitters.Remove(emitter);
            if (emitter is CompositeEmitter composite)
            {
                composite.Host = null;
            }
            else
            {
                _DepthLayers[emitter.Depth].Remove(emitter);
                if (_DepthLayers[emitter.Depth].IsEmpty)
                {
                    _DepthLayers.Remove(emitter.Depth);
                }
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
                layer.Draw(target, states);
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
    }
}