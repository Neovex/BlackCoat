using System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Very basic Emitter that continuously emits texture particles when triggered.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.ITriggerEmitter" />
    /// <seealso cref="BlackCoat.ParticleSystem.TextureEmitter" />
    public sealed class BasicTextureEmitter : TextureEmitter, ITriggerEmitter
    {
        private static readonly Guid _GUID = typeof(BasicTextureParticle).GUID;
        public override Guid ParticleTypeGuid => _GUID;
        private Single _SpawnTimer;


        /// <summary>
        /// Gets a value indicating whether this instance is currently triggered.
        /// </summary>
        public Boolean IsTriggered { get; private set; }
        /// <summary>
        /// The amount of particles that should be emitted during spawn phase.
        /// </summary>
        public Int32 ParticlesPerSpawn { get; set; }
        /// <summary>
        /// Particle animation information for particle initialization.
        /// </summary>
        public TextureParticleAnimationInfo ParticleInfo { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BasicTextureEmitter"/> is looping.
        /// This determines if the emitter needs to be re-triggered or runs continuously.
        /// </summary>
        public Boolean Loop { get; set; }
        /// <summary>
        /// Only relevant when loop = true. The spawn rate defines the time between each spawn phases.
        /// </summary>
        public Single SpawnRate { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="BasicTextureEmitter" /> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="texture">The texture for all particles.</param>
        /// <param name="particlesPerSpawn">The amount of particles that should be emitted during spawn phase.</param>
        /// <param name="info">Particle animation information.</param>
        /// <param name="loop">Determines if the emitter needs to be re-triggered or runs continuously.</param>
        /// <param name="spawnrate">Only relevant when loop = true. The spawn rate defines the time between each spawn phases.</param>
        /// <param name="blendMode">Optional particle blend mode. Defaults to Alpha Blending.</param>
        /// <param name="depth">The optional hierarchical depth.</param>
        public BasicTextureEmitter(Core core, Texture texture, Int32 particlesPerSpawn, TextureParticleAnimationInfo info,
                                   Boolean loop = false, Single spawnrate = 0, BlendMode? blendMode = null, int depth = 0) :
                                   base(core, texture, blendMode ?? BlendMode.Alpha, depth)
        {
            ParticlesPerSpawn = particlesPerSpawn;
            ParticleInfo = info ?? throw new ArgumentNullException(nameof(info));
            Loop = loop;
            SpawnRate = spawnrate;
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
                    _SpawnTimer = SpawnRate;
                    IsTriggered = Loop;

                    for (int i = 0; i < ParticlesPerSpawn; i++)
                    {
                        var particle = RetrieveFromCache() as BasicTextureParticle ?? new BasicTextureParticle(_Core);
                        particle.Initialize(Texture, Position, ParticleInfo);
                        AddParticle(particle, ParticleInfo.TTL);
                    }
                }
            }
        }
    }
}