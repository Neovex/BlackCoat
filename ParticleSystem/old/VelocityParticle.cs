using System;
using SFML.Graphics;
using BlackCoat.Entities;
using SFML.Window;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class VelocityParticle : Graphic
    {
        // Variables #######################################################################
        protected OldEmitter _Emitter;


        // Properties ######################################################################
        /// <summary>
        /// Particle Texture
        /// </summary>
        public new Texture Texture
        {
            set
            {
                base.Texture = value;
                if(value != null) Origin = new Vector2f(value.Size.X / 2, value.Size.Y / 2);
            }
        }

        /// <summary>
        /// Determines if the Particle is alive. Read Only.
        /// </summary>
        public virtual Boolean Alive { get { return TTL > 0; } }

        /// <summary>
        /// Remaining Lifetime of the Particle. Read and Write.
        /// </summary>
        public virtual Single TTL { get; set; }

        /// <summary>
        /// Movement Speed of the Particle. Read and Write.
        /// </summary>
        public virtual Single Speed { get; set; }

        /// <summary>
        /// Speed Modifier of the Particle. Read and Write.
        /// </summary>
        public virtual Single SpeedVelocity { get; set; }

        /// <summary>
        /// Movement Direction of the Particle. Read and Write.
        /// </summary>
        public virtual Single Direction { get; set; }

        /// <summary>
        /// Direction Modifier of the Particle. Read and Write.
        /// </summary>
        public virtual Single DirectionVelocity { get; set; }

        /// <summary>
        /// Alphablending of the Particle. Read and Write.
        /// </summary>
        public virtual Single Blend { get; set; }

        /// <summary>
        /// Particle Rotation. Read and Write.
        /// </summary>
        public virtual Single RotationRate { get; set; }

        /// <summary>
        /// Particle Scale. Read and Write.
        /// </summary>
        public virtual Single ScaleTransform { get; set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new  Instance of the Particle class.
        /// </summary>
        /// <param name="emitter">Parent Emitter of the Particle</param>
        /// <param name="core">BlackCoat Engine Core</param>
        public VelocityParticle(OldEmitter emitter, Core core):base(core)
        {
            if (emitter == null) throw new ArgumentNullException("emitter");
            _Emitter = emitter;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the Particle
        /// </summary>
        /// <param name="deltaT">Current gametime</param>
        public override void Update(Single deltaT)
        {
            if (Alive)
            {
                // TTL
                TTL -= deltaT;

                // Movement
                var pos = Position;
                var mv = VectorExtensions.VectorFromAngle(Direction);
                pos.X += mv.X * Speed * deltaT;
                pos.Y += mv.Y * Speed * deltaT;
                Position = pos;

                //Blending
                Alpha += Blend * deltaT;

                //Rotation
                Rotation += RotationRate * deltaT;

                //Scaling
                var scale = Scale;
                scale.X += ScaleTransform * deltaT;
                scale.Y += ScaleTransform * deltaT;
                Scale = scale;

                //Velocity calculation
                Speed += SpeedVelocity * deltaT;
                Direction += DirectionVelocity * deltaT;
            }
            else
            {
                _Emitter.DeactivateParticle(this);
            }
        }
    }
}