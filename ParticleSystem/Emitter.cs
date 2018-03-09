using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using BlackCoat.Entities;
using SFML.Window;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class Emitter:Container
    {
        // Statics #########################################################################
        public static Int32 ACTIVE_PARTICLES = 0;
        public static Int32 PARTICLE_LIMIT = 500;


        // Variables #######################################################################
        // System
        protected Queue<Particle> _InactiveParticles = new Queue<Particle>();
        private float _SpawnTimer;


        // Properties ######################################################################
        /// <summary>
        /// Determines if the Emitter has been triggered
        /// </summary>
        public virtual Boolean IsTriggered { get; protected set; }
        // Spawn Infos
        public virtual Vector2f TTLRange { get; set; }
        public virtual Vector2i ParticlesPerSpawn { get; set; }
        public virtual Single SpawnRate { get; set; }
        // Particle Ranges
        public virtual Container SpawnContainer { get; set; }
        public virtual Texture ParticleTexture { get; set; }
        public virtual IntRect SpawnArea { get; set; }
        public virtual Vector2f SpeedRange { get; set; }
        public virtual Vector2f SpeedVelocityRange { get; set; }
        public virtual Vector2f DirectionRange { get; set; }
        public virtual Vector2f DirectionVelocityRange { get; set; }
        public virtual Vector2f AlphaRange { get; set; }
        public virtual Vector2f BlendRange { get; set; }
        public virtual Vector2f AngleRange { get; set; }
        public virtual Vector2f RotationRange { get; set; }
        public virtual Vector2f ScaleRange { get; set; }
        public virtual Vector2f TransformRange { get; set; }
        public virtual BlendMode ParticleBlendMode { get; set; }

        // CTOR ############################################################################
        public Emitter(Core core):base(core)
        {
            TTLRange = new Vector2f(1, 1);
            ParticlesPerSpawn = new Vector2i(1, 1);
            SpawnRate = 1;
            SpawnContainer = this;
            AlphaRange = new Vector2f(1, 1);
            ScaleRange = new Vector2f(1, 1);
            ParticleBlendMode = BlendMode.Alpha;
        }


        // Methods #########################################################################
        /// <summary>
        /// Notifies the Emitter to begin emitting particles.
        /// Can be overridden by derived classes.
        /// </summary>
        public virtual void Trigger()
        {
            IsTriggered = true;
        }

        /// <summary>
        /// Notifies the Emitter that the specified particle exceeded its TTL.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="particle">The "dead" particle</param>
        public virtual void DeactivateParticle(Particle particle)
        {
            particle.Parent.RemoveChild(particle);
            _InactiveParticles.Enqueue(particle);
            ACTIVE_PARTICLES--;
        }

        /// <summary>
        /// Updates the Emitter.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public override void Update(Single deltaT)
        {
            // Update Self
            _SpawnTimer += deltaT;
            if (IsTriggered && _SpawnTimer >= SpawnRate)
            {
                _SpawnTimer = 0;
                Particle p;
                int particleCount = _Core.Random.Next(ParticlesPerSpawn.X, ParticlesPerSpawn.Y);
                for (int i = 0; i < particleCount; i++)
                {
                    if(_InactiveParticles.Count > 0) p = _InactiveParticles.Dequeue();
                    else p = new Particle(this, _Core);
                    if (!SpawnContainer.AddChild(p)) throw new Exception("invalid spawn container");
                    var scale = _Core.Random.NextFloat(ScaleRange.X, ScaleRange.Y);
                    p.Scale = new Vector2f(scale, scale);
                    p.Texture = ParticleTexture;
                    p.TTL = _Core.Random.NextFloat(TTLRange.X, TTLRange.Y);
                    p.Position = new Vector2f(_Core.Random.NextFloat(SpawnArea.Left, SpawnArea.Width), _Core.Random.NextFloat(SpawnArea.Top, SpawnArea.Height));
                    p.Speed = _Core.Random.NextFloat(SpeedRange.X, SpeedRange.Y);
                    p.SpeedVelocity = _Core.Random.NextFloat(SpeedVelocityRange.X, SpeedVelocityRange.Y);
                    p.Direction = _Core.Random.NextFloat(DirectionRange.X, DirectionRange.Y);
                    p.DirectionVelocity = _Core.Random.NextFloat(DirectionVelocityRange.X, DirectionVelocityRange.Y);
                    p.Alpha = _Core.Random.NextFloat(AlphaRange.X, AlphaRange.Y);
                    p.Blend = _Core.Random.NextFloat(BlendRange.X, BlendRange.Y);
                    p.Rotation = _Core.Random.NextFloat(AngleRange.X, AngleRange.Y);
                    p.RotationRate = _Core.Random.NextFloat(RotationRange.X, RotationRange.Y);
                    p.ScaleTransform = _Core.Random.NextFloat(TransformRange.X, TransformRange.Y);
                    p.BlendMode = ParticleBlendMode;
                    ACTIVE_PARTICLES++;
                }
            }
            // Update base
            base.Update(deltaT);
        }
    }
}