using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class BasicTextureEmitter : TextureEmitter
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

        public BasicTextureEmitter(Core core, Texture texture, BlendMode blendMode, int depth = 0) : base(core, texture, blendMode, depth)
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
                        var particle = RetrieveFromCache() as BasicTextureParticle ?? new BasicTextureParticle(_Core);
                        particle.Initialize(Texture, Position, Origin, Scale, Rotation, Color, Alpha, Velocity, Acceleration, RotationVelocity, Blending);
                        AddParticle(particle);
                    }
                }
            }
        }
    }
}