using System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Generic Emitter implementation.
    /// </summary>
    /// <typeparam name="Tparticle">The type of the spawned particle.</typeparam>
    /// <typeparam name="TInfo">The type of the initialization information.</typeparam>
    /// <seealso cref="BlackCoat.ParticleSystem.ITriggerEmitter" />
    /// <seealso cref="BlackCoat.ParticleSystem.EmitterBase" />
    public class Emitter<Tparticle, TInfo> : EmitterBase, ITriggerEmitter where Tparticle : ParticleBase, IInitializableByInfo<TInfo> where TInfo : PixelParticleInitializationInfo
    {
        private static readonly Guid _GUID = typeof(Tparticle).GUID;
        public override Guid ParticleTypeGuid => _GUID;


        protected Single _SpawnTimer;


        /// <summary>
        /// Gets a value indicating whether this instance is currently triggered.
        /// </summary>
        public Boolean IsTriggered { get; protected set; }
        /// <summary>
        /// Particle animation information for particle initialization.
        /// </summary>
        public TInfo ParticleInfo { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Emitter" /> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="info">The optional particle animation information.</param>
        /// <param name="primitiveType">Type of the particle primitive.</param>
        /// <param name="blendMode">The particle blend mode.</param>
        /// <param name="texture">Optional Texture to be mapped onto the vertices.</param>
        /// <param name="depth">The optional hierarchical depth.</param>
        public Emitter(Core core, TInfo info, PrimitiveType primitiveType, BlendMode? blendMode, Texture texture = null, int depth = 0) : 
                       base(core, depth, primitiveType, blendMode ?? BlendMode.Alpha, texture)
        {
            ParticleInfo = info ?? throw new ArgumentNullException(nameof(info));
        }


        /// <summary>
        /// Triggers the emitter. Causing it to start emitting particles.
        /// </summary>
        public virtual void Trigger()
        {
            IsTriggered = true;
        }

        /// <summary>
        /// Updates Emitter logic.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        protected override void Update(float deltaT)
        {
            if (IsTriggered)
            {
                _SpawnTimer -= deltaT;
                if (_SpawnTimer < 0)
                {
                    _SpawnTimer = ParticleInfo.SpawnRate;
                    IsTriggered = ParticleInfo.Loop;

                    var amount = ParticleInfo.ParticlesPerSpawn;
                    for (int i = 0; i < amount; i++)
                    {
                        var particle = RetrieveFromCache() as Tparticle ?? CreateParticle();
                        InitializeParticle(particle, i);
                        AddParticle(particle, ParticleInfo.TTL);
                    }
                }
            }
        }

        /// <summary>
        /// When not overridden creates a particle using the default BaseParticle constructor with one Core parameter.
        /// </summary>
        /// <returns>New Particle</returns>
        protected virtual Tparticle CreateParticle() => (Tparticle)Activator.CreateInstance(typeof(Tparticle), new object[] { _Core });

        /// <summary>
        /// Initializes a particle.
        /// </summary>
        /// <param name="particle">The particle to initialize.</param>
        /// <param name="index">The current index of the current spawn sequence.</param>
        protected virtual void InitializeParticle(IInitializableByInfo<TInfo> particle, int index) => particle.Initialize(Position, ParticleInfo);
    }
}