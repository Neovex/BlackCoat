using System;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class BasicPixelEmitter : PixelEmitter
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

        public BasicPixelEmitter(Core core, int depth = 0):base(core, depth)
        {
        }
        

        public void Trigger()
        {
            IsTriggered = true;
        }

        protected override void UpdateInternal(float deltaT)
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