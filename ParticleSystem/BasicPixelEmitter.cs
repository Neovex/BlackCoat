using System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Very basic Emitter that continuously emits pixel particles when triggered.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.ITriggerEmitter" />
    /// <seealso cref="BlackCoat.ParticleSystem.PixelEmitter" />
    public sealed class BasicPixelEmitter : BaseEmitter, ITriggerEmitter
    {
        private static readonly Guid _GUID = typeof(BasicPixelParticle).GUID;
        public override Guid ParticleTypeGuid => _GUID;
        private Single _SpawnTimer;


        /// <summary>
        /// Gets a value indicating whether this instance is currently triggered.
        /// </summary>
        public Boolean IsTriggered { get; private set; }
        /// <summary>
        /// Particle animation information for particle initialization.
        /// </summary>
        public ParticleAnimationInfo ParticleInfo { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="BasicPixelEmitter" /> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="info">The optional particle animation information.</param>
        /// <param name="depth">The optional hierarchical depth.</param>
        public BasicPixelEmitter(Core core, ParticleAnimationInfo info, int depth = 0) : base(core, depth, PrimitiveType.Points, BlendMode.Alpha)
        {
            ParticleInfo = info ?? throw new ArgumentNullException(nameof(info));
        }


        /// <summary>
        /// Triggers the emitter. Causing it to start emitting particles.
        /// </summary>
        public void Trigger()
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
                        var particle = RetrieveFromCache() as BasicPixelParticle ?? new BasicPixelParticle(_Core);
                        particle.Initialize(Position, ParticleInfo);
                        AddParticle(particle, ParticleInfo.TTL);
                    }
                }
            }
        }
    }
}