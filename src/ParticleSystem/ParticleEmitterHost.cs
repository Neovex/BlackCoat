using System;
using System.Linq;
using System.Collections.Generic;

using SFML.System;
using SFML.Graphics;

using BlackCoat.Entities;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// This class represents the bridge between the Particle System and the entity scene graph.
    /// </summary>
    /// <seealso cref="BlackCoat.Entities.EntityBase" />
    public sealed class ParticleEmitterHost : EntityBase
    {
        // Variables #######################################################################
        private bool _Disposed;
        private readonly List<EmitterBase> _Emitters;
        private readonly SortedList<int, List<ParticleVertexRenderer>> _DepthLayers;


        // Properties ######################################################################
        /// <summary>
        /// The Position of the <see cref="ParticleEmitterHost"/> has no effect on emitters.
        /// Use the appropriate Position property of the corresponding <see cref="EmitterBase"/>.
        /// </summary>
        public override Vector2f Position { get => default; set { } }

        /// <summary>
        /// The Rotation of the <see cref="ParticleEmitterHost"/> has no effect on emitters.
        /// Use the appropriate Rotation property of the corresponding <see cref="ParticleSpawnInfo"/> of each emitter.
        /// </summary>
        public override float Rotation { get => default; set { } }

        /// <summary>
        /// The Origin of the <see cref="ParticleEmitterHost"/> has no effect on emitters.
        /// Use the appropriate Origin property of the corresponding <see cref="TexturedSpawnInfo"/> of each emitter.
        /// </summary>
        public override Vector2f Origin { get => default; set { } }

        /// <summary>
        /// The Scale of the <see cref="ParticleEmitterHost"/> has no effect on emitters.
        /// Use the appropriate Scale property of the corresponding <see cref="TexturedSpawnInfo"/> of each emitter.
        /// </summary>
        public override Vector2f Scale { get => new Vector2f(1, 1); set { } }

        /// <summary>
        /// The Color of the <see cref="ParticleEmitterHost"/> has no effect on emitters.
        /// Use the appropriate color property of the corresponding <see cref="ParticleSpawnInfo"/> of each emitter.
        /// </summary>
        public override Color Color { get; set; }

        /// <summary>
        /// Determines whether this <see cref="ParticleEmitterHost" /> is destroyed.
        /// </summary>
        public override bool Disposed => _Disposed;

        internal IEnumerable<EmitterBase> Emitters => _Emitters;
        internal IEnumerable<ParticleVertexRenderer> DepthLayers => _DepthLayers.Values.SelectMany(l => l);


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleEmitterHost"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        public ParticleEmitterHost(Core core) : base(core)
        {
            _Emitters = new List<EmitterBase>();
            _DepthLayers = new SortedList<int, List<ParticleVertexRenderer>>();
        }


        // Methods #########################################################################
        /// <summary>
        /// Adds an emitter to the host.
        /// </summary>
        /// <param name="emitter">The emitter to add.</param>
        /// <exception cref="ArgumentNullException">emitter</exception>
        public void AddEmitter(EmitterBase emitter)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(ParticleEmitterHost));
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
                vertexRenderer.AssociatedEmitterCount++;
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
            if (Disposed) throw new ObjectDisposedException(nameof(ParticleEmitterHost));
            if (emitter == null) throw new ArgumentNullException(nameof(emitter));

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
                vertexRenderer.AssociatedEmitterCount--;
                if (vertexRenderer.AssociatedEmitterCount == 0) layer.Remove(vertexRenderer);
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

        /// <summary>
        /// Releases managed and unmanaged resources.
        /// </summary>
        /// <param name="disposeManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposeManaged)
        {
            if (!Disposed)
            {
                base.Dispose(disposeManaged);
                if (disposeManaged)
                {
                    _Emitters.Clear();
                    _DepthLayers.Clear();
                }
                _Disposed = true;
            }
        }
    }
}