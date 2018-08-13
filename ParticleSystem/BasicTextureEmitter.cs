using System;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Very basic Emitter that continuously emits texture particles when triggered.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.TextureEmitter" />
    public sealed class BasicTextureEmitter : TextureEmitter
    {
        private static readonly Guid _GUID = typeof(BasicTextureParticle).GUID;

        private Single _SpawnTimer;

        // Emitter / Particle Infos
        public override Guid ParticleTypeGuid => _GUID;
        public Vector2f Velocity { get; set; }
        public Vector2f Acceleration { get; set; }
        public float  RotationVelocity { get; set; }
        public float  Blending { get; set; }
        // Spawn Infos
        public Single SpawnRate { get; set; }
        public Int32 ParticlesPerSpawn { get; set; }
        public Boolean Loop { get; set; }
        public bool IsTriggered { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="BasicTextureEmitter"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="texture">The texture for all particles.</param>
        /// <param name="blendMode">The particle blend mode.</param>
        /// <param name="depth">The optional hierarchical depth.</param>
        public BasicTextureEmitter(Core core, Texture texture, BlendMode blendMode, int depth = 0) : base(core, texture, blendMode, depth)
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
                        var particle = RetrieveFromCache() as BasicTextureParticle ?? new BasicTextureParticle(_Core);
                        particle.Initialize(Texture, Position, Origin, Scale, Rotation, Color, Alpha, Velocity, Acceleration, RotationVelocity, Blending);
                        AddParticle(particle);
                    }
                }
            }
        }
    }
}