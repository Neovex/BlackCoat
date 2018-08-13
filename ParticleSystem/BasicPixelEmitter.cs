using System;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Very basic Emitter that continuously emits pixel particles when triggered.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.PixelEmitter" />
    public sealed class BasicPixelEmitter : PixelEmitter
    {
        private static readonly Guid _GUID = typeof(BasicPixelParticle).GUID;

        private Single _SpawnTimer;

        // Emitter / Particle Infos
        public override Guid ParticleTypeGuid => _GUID;
        public Vector2f Velocity { get; set; }
        public Vector2f Acceleration { get; set; }
        // Spawn Infos
        public Single SpawnRate { get; set; }
        public Int32 ParticlesPerSpawn { get; set; }
        public Boolean Loop { get; set; }
        public bool IsTriggered { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="BasicPixelEmitter"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="depth">The optional hierarchical depth.</param>
        public BasicPixelEmitter(Core core, int depth = 0):base(core, depth)
        {
        }


        /// <summary>
        /// Triggers this instance. Causing it to start emitting particles.
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
                        var particle = RetrieveFromCache() as BasicPixelParticle ?? new BasicPixelParticle(_Core);
                        particle.Initialize(Position, Color, Velocity, Acceleration);
                        AddParticle(particle);
                    }
                }
            }
        }
    }
}