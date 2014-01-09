﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    public class Emitter:Container
    {
        // Constants #######################################################################
        public static Int32 ACTIVE_PARTICLES = 0;
        public static Int32 PARTICLE_LIMIT = 500;


        // Variables #######################################################################
        // System
        protected Queue<Particle> _InactiveParticles = new Queue<Particle>();
        protected Boolean _IsTriggered = false;
        // Spawn Infos
        protected Vector2 _TTLRange = new Vector2(1, 1);
        protected Vector2 _ParticlesPerSpawn = new Vector2(1, 1);
        protected Single _SpawnRate = 1;
        protected Single _SpawnTimer = 0;
        // Particle Ranges
        protected Container _SpawnContainer;
        protected Image _ParticleTexture = null;
        protected IntRect _SpawnArea = new IntRect(0, 0, 0, 0);
        protected Vector2 _SpeedRange = new Vector2();
        protected Vector2 _VelocityRange = new Vector2();
        protected Vector2 _DirectionRange = new Vector2();
        protected Vector2 _DirectionVelocityRange = new Vector2();
        protected Vector2 _AlphaRange = new Vector2(1, 1);
        protected Vector2 _BlendRange = new Vector2();
        protected Vector2 _AngleRange = new Vector2();
        protected Vector2 _RotationRange = new Vector2();
        protected Vector2 _ScaleRange = new Vector2(1, 1);
        protected Vector2 _TransformRange = new Vector2();
        private BlendMode _ParticleBlendMode = BlendMode.None;


        // Properties ######################################################################
        /// <summary>
        /// Determines if the Emitter as Triggert
        /// </summary>
        public virtual Boolean IsTriggered { get { return _IsTriggered; } }
        // Spawn Infos
        public virtual Vector2 TTLRange
        {
            get { return _TTLRange; }
            set { _TTLRange = value; }
        }

        public virtual Vector2 ParticlesPerSpawn
        {
            get { return _ParticlesPerSpawn; }
            set { _ParticlesPerSpawn = value; }
        }

        public virtual Single SpawnRate
        {
            get { return _SpawnRate; }
            set { _SpawnRate = value; }
        }
        // Particle Ranges
        public virtual Container SpawnContainer
        {
            get { return _SpawnContainer; }
            set { _SpawnContainer = value; }
        }

        public virtual Image ParticleTexture
        {
            get { return _ParticleTexture; }
            set { _ParticleTexture = value; }
        }

        public virtual IntRect SpawnArea
        {
            get { return _SpawnArea; }
            set { _SpawnArea = value; }
        }

        public virtual Vector2 SpeedRange
        {
            get { return _SpeedRange; }
            set { _SpeedRange = value; }
        }

        public virtual Vector2 SpeedVelocityRange
        {
            get { return _VelocityRange; }
            set { _VelocityRange = value; }
        }

        public virtual Vector2 DirectionRange
        {
            get { return _DirectionRange; }
            set { _DirectionRange = value; }
        }

        public virtual Vector2 DirectionVelocityRange
        {
            get { return _DirectionVelocityRange; }
            set { _DirectionVelocityRange = value; }
        }

        public virtual Vector2 AlphaRange
        {
            get { return _AlphaRange; }
            set { _AlphaRange = value; }
        }

        public virtual Vector2 BlendRange
        {
            get { return _BlendRange; }
            set { _BlendRange = value; }
        }

        public virtual Vector2 AngleRange
        {
            get { return _AngleRange; }
            set { _AngleRange = value; }
        }

        public virtual Vector2 RotationRange
        {
            get { return _RotationRange; }
            set { _RotationRange = value; }
        }

        public virtual Vector2 ScaleRange
        {
            get { return _ScaleRange; }
            set { _ScaleRange = value; }
        }

        public virtual Vector2 TransformRange
        {
            get { return _TransformRange; }
            set { _TransformRange = value; }
        }

        public virtual BlendMode ParticleBlendMode
        {
            get { return _ParticleBlendMode; }
            set { _ParticleBlendMode = value; }
        }

        // CTOR ############################################################################
        public Emitter(Core core):base(core)
        {
            _SpawnContainer = this;
        }


        // Methods #########################################################################
        /// <summary>
        /// Notifies the Emitter to begin emitting particles.
        /// Can be overriden by derived classes.
        /// </summary>
        public virtual void Trigger()
        {
            _IsTriggered = true;
        }

        /// <summary>
        /// Notifies the Emitter that the specified particle exceedet its TTL.
        /// Can be overriden by derived classes.
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
        /// Can be overriden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current gametime</param>
        public override void Update(Single deltaT)
        {
            // Update Self
            _SpawnTimer += deltaT;
            if (_IsTriggered && _SpawnTimer >= _SpawnRate)
            {
                _SpawnTimer = 0;
                Particle p;
                int particleCount = _Core.Random.Next((int)_ParticlesPerSpawn.X, (int)_ParticlesPerSpawn.Y);
                for (int i = 0; i < particleCount; i++)
                {
                    if(_InactiveParticles.Count > 0) p = _InactiveParticles.Dequeue();
                    else p = new Particle(this, _Core);
                    if (!_SpawnContainer.AddChild(p)) throw new Exception("invalid spawn container");
                    var scale = _Core.Random.NextFloat(_ScaleRange.X, _ScaleRange.Y);
                    p.Scale = new Vector2(scale, scale);
                    p.Image = _ParticleTexture;
                    p.TTL = _Core.Random.NextFloat(_TTLRange.X, _TTLRange.Y);
                    p.Position = new Vector2(_Core.Random.NextFloat(_SpawnArea.Left, _SpawnArea.Right),
                                             _Core.Random.NextFloat(_SpawnArea.Top, _SpawnArea.Bottom));
                    p.Speed = _Core.Random.NextFloat(_SpeedRange.X, _SpeedRange.Y);
                    p.SpeedVelocity = _Core.Random.NextFloat(_VelocityRange.X, _VelocityRange.Y);
                    p.Direction = _Core.Random.NextFloat(_DirectionRange.X, _DirectionRange.Y);
                    p.DirectionVelocity = _Core.Random.NextFloat(_DirectionVelocityRange.X, _DirectionVelocityRange.Y);
                    p.Alpha = _Core.Random.NextFloat(_AlphaRange.X, _AlphaRange.Y);
                    p.Blend = _Core.Random.NextFloat(_BlendRange.X, _BlendRange.Y);
                    p.Rotation = _Core.Random.NextFloat(_AngleRange.X, _AngleRange.Y);
                    p.RotationRate = _Core.Random.NextFloat(_RotationRange.X, _RotationRange.Y);
                    p.Transform = _Core.Random.NextFloat(_TransformRange.X, _TransformRange.Y);
                    p.BlendMode = _ParticleBlendMode;
                    ACTIVE_PARTICLES++;
                }

                //TODO : UNHACK
                _IsTriggered = false;
            }
            // Update Childs
            base.Update(deltaT);
        }
    }
}