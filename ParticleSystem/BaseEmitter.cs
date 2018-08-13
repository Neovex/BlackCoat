using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Emitter base class <seealso cref="Particles.cd"/>
    /// </summary>
    /// <seealso cref="BlackCoat.BlackCoatBase" />
    public abstract class BaseEmitter : BlackCoatBase
    {
        static BaseEmitter() => INSTANCE_POOL = new Dictionary<Guid, Stack<BaseParticle>>();
        private static Dictionary<Guid, Stack<BaseParticle>> INSTANCE_POOL { get; }

        private VertexRenderer _VertexRenderer;
        private List<BaseParticle> _Particles;


        /// <summary>
        /// Gets the depth of this instance within the <see cref="ParticleEmitterHost"/> hierarchy
        /// </summary>
        public int Depth { get; }
        /// <summary>
        /// Gets or sets the default Particle Lifetime.
        /// </summary>
        public float DefaultTTL { get; set; }
        /// <summary>
        /// Gets or sets the position of this instance.
        /// </summary>
        public virtual Vector2f Position { get; set; }
        /// <summary>
        /// Gets or sets the rotation of this instance.
        /// </summary>
        public virtual float Rotation { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        public bool IsInitialized => _VertexRenderer != null;
        public abstract Guid ParticleTypeGuid { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEmitter"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="depth">The optional hierarchical depth.</param>
        public BaseEmitter(Core core, int depth = 0) : base(core)
        {
            Depth = depth;
            DefaultTTL = 1;
            _Particles = new List<BaseParticle>();
        }

        /// <summary>
        /// Adds a particle to the emitter. With default TTL.
        /// </summary>
        /// <param name="particle">The particle to add.</param>
        protected void AddParticle(BaseParticle particle)
        {
            AddParticle(particle, DefaultTTL);
        }
        /// <summary>
        /// Adds a particle to the emitter.
        /// </summary>
        /// <param name="particle">The particle to add.</param>
        /// <param name="ttl">The TTL of the particle.</param>
        protected void AddParticle(BaseParticle particle, float ttl)
        {
            particle.Initialize(_VertexRenderer.Reserve(), ttl);
            _Particles.Add(particle);
        }

        /// <summary>
        /// Updates the Emitter and its particles.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        internal virtual unsafe void UpdateInternal(Single deltaT)
        {
            if (!IsInitialized) throw new InvalidStateException();

            Update(deltaT);

            fixed (Vertex* vPtr = _VertexRenderer.Verticies)
            {
                for (int i = _Particles.Count - 1; i >= 0; i--)
                {
                    if (_Particles[i].Update(deltaT, vPtr))
                    {
                        _VertexRenderer.Free(_Particles[i].Index);
                        _Particles[i].Release(vPtr);
                        AddToCache(_Particles[i]);
                        // O(1) Swap Removal - faster removal but destroys order which is not important here
                        _Particles[i] = _Particles[_Particles.Count - 1];
                        _Particles.RemoveAt(_Particles.Count - 1);
                    }
                }
            }
        }

        /// <summary>
        /// Updates only the Emitter logic.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        protected abstract void Update(float deltaT);

        /// <summary>
        /// Initializes the Emitter assigning it with specified vertex renderer.
        /// </summary>
        /// <param name="vertexRenderer">The vertex renderer this Emitter should use.</param>
        internal void Initialize(VertexRenderer vertexRenderer)
        {
            _VertexRenderer = vertexRenderer ?? throw new ArgumentNullException(nameof(vertexRenderer));
        }

        /// <summary>
        /// Recycles this instance and clears all Particles.
        /// </summary>
        internal unsafe void Cleanup()
        {
            fixed (Vertex* vPtr = _VertexRenderer.Verticies)
            {
                for (int i = 0; i < _Particles.Count; i++)
                {
                    _Particles[i].Release(vPtr);
                    AddToCache(_Particles[i]);
                }
            }
            _Particles.Clear();
        }


        // Cache Handling        
        /// <summary>
        /// Adds to a particle to the instance cache.
        /// </summary>
        /// <param name="particle">The particle.</param>
        protected void AddToCache(BaseParticle particle)
        {
            if (!INSTANCE_POOL.ContainsKey(ParticleTypeGuid))
            {
                INSTANCE_POOL.Add(ParticleTypeGuid, new Stack<BaseParticle>());
            }
            INSTANCE_POOL[ParticleTypeGuid].Push(particle);
        }

        /// <summary>
        /// Retrieves a particle from cache.
        /// </summary>
        /// <returns>A particle of the type this emitter is associated with.</returns>
        protected BaseParticle RetrieveFromCache()
        {
            if (!INSTANCE_POOL.ContainsKey(ParticleTypeGuid)) return null;
            var pool = INSTANCE_POOL[ParticleTypeGuid];
            if (pool.Count == 0) return null;
            return pool.Pop();
        }
    }
}