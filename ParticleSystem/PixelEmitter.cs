using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class PixelEmitter : BaseEmitter
    {
        private Single _SpawnCounter;
        private VertexRenderer _VertexRenderer;

        internal VertexRenderer VertexRenderer
        {
            get => _VertexRenderer;
            set
            {
                _VertexRenderer = value;
                if (_VertexRenderer == null) Destroy();
            }
        }

        public int Depth { get; }
        public override Vector2f Position { get; set; }
        public override float Rotation { get; set; }

        public Vector2f Velocity { get; set; }
        public Vector2f Acceleration { get; set; }
        public Color Tint { get; set; }



        // Spawn Infos
        public virtual Single SpawnRate { get; set; }
        public virtual Int32 ParticlesPerSpawn { get; set; }
        public virtual Boolean Loop { get; set; }
        // Particle Ranges
        public virtual Single DefaultTTL { get; set; }
        public virtual Vector2f DefaultVelocity { get; set; }


        public PixelEmitter(int depth = 0)
        {
            Depth = depth;
        }
        

        protected override void Triggered()
        {
            if (VertexRenderer == null) throw new InvalidStateException();
        }

        protected override void UpdateInternal(float deltaT)
        {
            if (IsTriggered)
            {
                _SpawnCounter -= deltaT;
                if (_SpawnCounter < 0)
                {
                    _SpawnCounter = SpawnRate;
                    for (int i = 0; i < ParticlesPerSpawn; i++)
                    {
                        var particle = GetParticle();/*RetrieveFromCache() ?? new BasicParticle();*/
                        particle.Initialize(_VertexRenderer, DefaultTTL, Position, Velocity, Acceleration, Tint);
                        //_Particles.Add(particle);
                    }
                    IsTriggered = Loop;
                }
            }
        }

        private BasicParticle GetParticle()
        {
            var particle = _Particles.FirstOrDefault(p => p.TTL <= 0);
            if (particle == null)
            {
                particle = new BasicParticle();
                _Particles.Add(particle);
            }
            return particle;
        }
    }
}