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
    public abstract class EmitterBase : BlackCoatBase
    {
        // Particle Pool ###################################################################
        static EmitterBase() => PARTICLE_CACHE = new Dictionary<Guid, Stack<ParticleBase>>();
        private static Dictionary<Guid, Stack<ParticleBase>> PARTICLE_CACHE { get; }


        // Variables #######################################################################
        private ParticleVertexRenderer _VertexRenderer;
        private List<ParticleBase> _Particles;


        // Properties ######################################################################
        /// <summary>
        /// Gets the particle type <see cref="Guid"/> this emitter is associated with.
        /// </summary>
        public abstract Guid ParticleTypeGuid { get; }
        
        /// <summary>
        /// Gets the depth of this instance within the <see cref="ParticleEmitterHost"/> hierarchy
        /// </summary>
        public int Depth { get; }
        
        /// <summary>
        /// Gets the type of the particle primitive.
        /// </summary>
        public PrimitiveType PrimitiveType { get; }
        
        /// <summary>
        /// Gets the particle blend mode.
        /// </summary>
        public BlendMode BlendMode { get; }
        
        /// <summary>
        /// Gets the texture mapped onto the vertices. Can be null.
        /// </summary>
        public Texture Texture { get; }
        
        /// <summary>
        /// Gets or sets the position of this instance.
        /// </summary>
        public virtual Vector2f Position { get; set; }
        
        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        public bool IsInitialized => _VertexRenderer != null;
        
        /// <summary>
        /// Parent Emitter when part of a composition
        /// </summary>
        public EmitterComposition Composition { get; internal set; }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitterBase" /> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="primitiveType">Type of the particle primitive.</param>
        /// <param name="texture">Optional Texture to be mapped onto the vertices.</param>
        public EmitterBase(Core core, PrimitiveType primitiveType, Texture texture = null) :
                           this(core, 0, primitiveType, BlendMode.Alpha, texture)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitterBase"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="depth">The depth defining the render hierarchy.</param>
        /// <param name="primitiveType">Type of the particle primitive.</param>
        /// <param name="blendMode">The particle blend mode.</param>
        /// <param name="texture">Optional Texture to be mapped onto the vertices.</param>
        public EmitterBase(Core core, int depth, PrimitiveType primitiveType, BlendMode blendMode, Texture texture = null) : base(core)
        {
            _Particles = new List<ParticleBase>();
            Depth = depth;
            PrimitiveType = primitiveType;
            BlendMode = blendMode;
            Texture = texture;
        }


        // Methods #########################################################################
        /// <summary>
        /// Adds a particle to the emitter.
        /// </summary>
        /// <param name="particle">The particle to add.</param>
        /// <param name="ttl">The particles maximum lifetime.</param>
        protected void AddParticle(ParticleBase particle, float ttl)
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
            if (!IsInitialized) throw new InvalidStateException("Emitter is not initialized");

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
                        // O(3) Swap Removal - faster removal but destroys order which isn't needed
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
        internal void Initialize(ParticleVertexRenderer vertexRenderer)
        {
            _VertexRenderer = vertexRenderer ?? throw new ArgumentNullException(nameof(vertexRenderer));
        }

        /// <summary>
        /// Recycles this instance and clears all Particles.
        /// </summary>
        internal virtual unsafe void Cleanup()
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


        #region Particle Cache        
        /// <summary>
        /// Adds to a particle to the instance cache.
        /// </summary>
        /// <param name="particle">The particle.</param>
        protected void AddToCache(ParticleBase particle)
        {
            if (!PARTICLE_CACHE.ContainsKey(ParticleTypeGuid))
            {
                PARTICLE_CACHE.Add(ParticleTypeGuid, new Stack<ParticleBase>());
            }
            PARTICLE_CACHE[ParticleTypeGuid].Push(particle);
        }

        /// <summary>
        /// Retrieves a particle from cache.
        /// </summary>
        /// <returns>A particle of the type this emitter is associated with.</returns>
        protected ParticleBase RetrieveFromCache()
        {
            if (!PARTICLE_CACHE.ContainsKey(ParticleTypeGuid)) return null;
            var pool = PARTICLE_CACHE[ParticleTypeGuid];
            if (pool.Count == 0) return null;
            return pool.Pop();
        }
        #endregion
    }
}